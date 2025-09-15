using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Infrastructure.Jobs;

public interface IHangfireJob<in TEvent> where TEvent : IHangfireEvent
{
    Task<bool> CheckSupportAsync(TEvent @event, CancellationToken cancellationToken = default);
    Task RunAsync(TEvent @event, CancellationToken cancellationToken = default);
}
