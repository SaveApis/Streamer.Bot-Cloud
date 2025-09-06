using FluentResults;
using MediatR;

namespace Utils.Mediator.Infrastructure.Queries.Stream;

public interface IStreamQuery<TResult> : IStreamRequest<Result<TResult>>;
