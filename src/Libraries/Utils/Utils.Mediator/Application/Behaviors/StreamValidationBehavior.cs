using System.Runtime.CompilerServices;
using FluentResults;
using MediatR;
using Utils.Mediator.Application.Errors;
using Utils.Validation.Infrastructure.Services;

namespace Utils.Mediator.Application.Behaviors;

public class StreamValidationBehavior<TRequest, TResponse>(IValidationService validationService) : IStreamPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    public async IAsyncEnumerable<Result<TResponse>> Handle(TRequest request, StreamHandlerDelegate<Result<TResponse>> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var requestValidationResult = await validationService.ValidateAsync(request, cancellationToken);
        if (!requestValidationResult.IsValid)
        {
            yield return new ValidationError<TRequest>(request, requestValidationResult.Errors.ToList().AsReadOnly());
            yield break;
        }
        
        await foreach (var response in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}
