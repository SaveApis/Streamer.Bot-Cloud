using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Commands.Stream;

public interface IStreamCommand<TResult> : IStreamRequest<Result<TResult>>;
