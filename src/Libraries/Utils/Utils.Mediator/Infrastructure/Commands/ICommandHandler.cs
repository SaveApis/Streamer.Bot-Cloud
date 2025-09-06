using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Commands;

public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>> where TRequest : ICommand<TResponse>;
