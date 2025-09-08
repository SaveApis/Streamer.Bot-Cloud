using Hangfire;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
public class RemoveRecurringEventJob(ILogger<IJob<RecurringEventFoundEvent>> logger, IRecurringJobManagerV2 manager) : BaseJob<RecurringEventFoundEvent>(logger)
{
    [HangfireJobName("{0}: Remove recurring event")]
    public override Task RunAsync(RecurringEventFoundEvent @event, CancellationToken cancellationToken = default)
    {
        manager.RemoveIfExists(@event.Summary.Id);

        return Task.CompletedTask;
    }
}
