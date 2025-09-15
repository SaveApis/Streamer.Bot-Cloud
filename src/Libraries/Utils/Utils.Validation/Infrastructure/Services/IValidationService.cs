using FluentValidation;
using FluentValidation.Results;

namespace Utils.Validation.Infrastructure.Services;

public interface IValidationService
{
    ValidationResult Validate<T>(T instance);
    ValidationResult Validate<T>(ValidationContext<T> context);

    Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateAsync<T>(ValidationContext<T> context, CancellationToken cancellationToken = default);
}
