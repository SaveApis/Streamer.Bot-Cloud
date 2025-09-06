using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
