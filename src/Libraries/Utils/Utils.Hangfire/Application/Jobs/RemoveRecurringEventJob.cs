using Hangfire;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
public class RemoveRecurringEventJob(ILogger<IHangfireJob<RemovableRecurringEventFoundEvent>> logger, IRecurringJobManagerV2 manager) : BaseHangfireJob<RemovableRecurringEventFoundEvent>(logger)
{
    [HangfireJobName("{0}: Remove recurring event")]
    public override Task RunAsync(RemovableRecurringEventFoundEvent @event, CancellationToken cancellationToken = default)
    {
        manager.RemoveIfExists(@event.Id);

        return Task.CompletedTask;
    }
}
