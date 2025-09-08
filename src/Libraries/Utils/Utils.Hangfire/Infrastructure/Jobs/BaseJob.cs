using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Infrastructure.Jobs;

public abstract class BaseJob<TEvent>(ILogger<IJob<TEvent>> logger) : IJob<TEvent> where TEvent : IHangfireEvent
{
    protected ILogger<IJob<TEvent>> Logger { get; } = logger;

    public virtual async Task<bool> CheckSupportAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(CheckSupport(@event));
    }
    protected virtual bool CheckSupport(TEvent @event)
    {
        return true;
    }

    public abstract Task RunAsync(TEvent @event, CancellationToken cancellationToken = default);
}
