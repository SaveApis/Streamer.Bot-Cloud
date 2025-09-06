using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Commands.Stream;

public interface IStreamCommandHandler<in TRequest, TResult> : IStreamRequestHandler<TRequest, Result<TResult>> where TRequest : IStreamCommand<TResult>;
