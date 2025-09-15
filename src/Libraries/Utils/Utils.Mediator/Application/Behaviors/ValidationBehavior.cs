using FluentResults;
using MediatR;
using Utils.Mediator.Application.Errors;
using Utils.Validation.Infrastructure.Services;

namespace Utils.Mediator.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidationService validationService) : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        var requestValidationResult = await validationService.ValidateAsync(request, cancellationToken);
        if (!requestValidationResult.IsValid)
        {
            return new ValidationError<TRequest>(request, requestValidationResult.Errors.ToList().AsReadOnly());
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
