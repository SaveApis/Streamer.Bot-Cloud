using FluentResults;
using FluentValidation.Results;

namespace Utils.Mediator.Application.Errors;

public class ValidationError<TType>(TType instance, IReadOnlyCollection<ValidationFailure> errors) : Error(
    $"""
     Validation failed for type '{typeof(TType).Name}'.

     Errors:
     {string.Join(Environment.NewLine, errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}"))}
     """
)
{
    public TType Instance { get; } = instance;
    public IReadOnlyCollection<ValidationFailure> Errors { get; } = errors;
}
