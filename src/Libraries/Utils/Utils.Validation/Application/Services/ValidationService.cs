using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Utils.Validation.Infrastructure.Services;

namespace Utils.Validation.Application.Services;

public class ValidationService(ILifetimeScope lifetimeScope) : IValidationService
{
    public ValidationResult Validate<T>(T instance)
    {
        return Validate(new ValidationContext<T>(instance));
    }
    public ValidationResult Validate<T>(ValidationContext<T> context)
    {
        var validators = lifetimeScope.Resolve<IEnumerable<IValidator<T>>>().ToList();
        if (validators.Count == 0)
        {
            var logger = lifetimeScope.Resolve<ILogger<ValidationService>>();
            logger.LogWarning("No validators found for type {Type}", typeof(T).FullName);

            return new ValidationResult();
        }

        var validationResults = validators.Select(v => v.Validate(context));

        return new ValidationResult(validationResults);
    }

    public async Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        return await ValidateAsync(new ValidationContext<T>(instance), cancellationToken);
    }
    public async Task<ValidationResult> ValidateAsync<T>(ValidationContext<T> context, CancellationToken cancellationToken = default)
    {
        var validators = lifetimeScope.Resolve<IEnumerable<IValidator<T>>>().ToList();
        if (validators.Count == 0)
        {
            var logger = lifetimeScope.Resolve<ILogger<ValidationService>>();
            logger.LogWarning("No validators found for type {Type}", typeof(T).FullName);

            return new ValidationResult();
        }

        var validationResultTasks = validators.Select(v => v.ValidateAsync(context, cancellationToken));
        var validationResults = await Task.WhenAll(validationResultTasks);

        return new ValidationResult(validationResults);
    }
}
