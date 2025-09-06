using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Queries.Stream;

public interface IStreamQueryHandler<in TRequest, TResult> : IStreamRequestHandler<TRequest, Result<TResult>> where TRequest : IStreamQuery<TResult>;
