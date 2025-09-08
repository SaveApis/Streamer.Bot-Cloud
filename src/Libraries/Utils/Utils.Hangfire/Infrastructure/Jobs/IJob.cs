using Hangfire.Server;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Infrastructure.Jobs;

public interface IJob<in TEvent> where TEvent : IHangfireEvent
{
    Task<bool> CheckSupportAsync(TEvent @event, CancellationToken cancellationToken = default);
    Task RunAsync(TEvent @event, CancellationToken cancellationToken = default);
}
