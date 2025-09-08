using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
public class CreateRecurringEventJob(ILogger<IJob<RecurringEventFoundEvent>> logger, IRecurringJobManagerV2 manager, IMediator mediator) : BaseJob<RecurringEventFoundEvent>(logger)
{
    protected override bool CheckSupport(RecurringEventFoundEvent @event)
    {
        return @event.Summary is { IsInCode: true, IsInHangfire: false };
    }

    [HangfireJobName("{0}: Create recurring event")]
    public override Task RunAsync(RecurringEventFoundEvent @event, CancellationToken cancellationToken = default)
    {
        var recurringJobOptions = new RecurringJobOptions()
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")
        };
        var summary = @event.Summary;
        var instance = summary.CodeInstance;
        if (instance == null)
        {
            Logger.LogError("The recurring event {RecurringEventId} has no code instance defined.", summary.Id);
            return Task.CompletedTask;
        }

        manager.AddOrUpdate(summary.Id, summary.CodeQueue.ToString().ToLowerInvariant(), () => Publish(instance, CancellationToken.None), () => summary.CodeCron, recurringJobOptions);

        return Task.CompletedTask;
    }

    [HangfireJobName("{0}: Publish recurring event")]
    public async Task Publish(IRecurringHangfireEvent recurringHangfireEvent, CancellationToken cancellationToken)
    {
        await mediator.Publish(recurringHangfireEvent, cancellationToken).ConfigureAwait(false);
    }
}
