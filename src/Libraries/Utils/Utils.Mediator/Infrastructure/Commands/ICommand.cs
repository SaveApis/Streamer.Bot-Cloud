using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Commands;

public interface ICommand<TResult> : IRequest<Result<TResult>>;
