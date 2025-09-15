using Microsoft.Extensions.Logging;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Infrastructure.Jobs;

public abstract class BaseHangfireJob<TEvent>(ILogger<IHangfireJob<TEvent>> logger) : IHangfireJob<TEvent> where TEvent : IHangfireEvent
{
    protected ILogger<IHangfireJob<TEvent>> Logger { get; } = logger;
    public virtual Task<bool> CheckSupportAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CheckSupport(@event));
    }
    protected virtual bool CheckSupport(TEvent @event)
    {
        return true;
    }

    public abstract Task RunAsync(TEvent @event, CancellationToken cancellationToken = default);
}
