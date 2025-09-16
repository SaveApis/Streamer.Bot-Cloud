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
public class CreateRecurringEventJob(ILogger<IHangfireJob<CreatableRecurringEventFoundEvent>> logger, IRecurringJobManagerV2 manager, IMediator mediator) : BaseHangfireJob<CreatableRecurringEventFoundEvent>(logger)
{
    [HangfireJobName("{0}: Create recurring event")]
    public override Task RunAsync(CreatableRecurringEventFoundEvent @event, CancellationToken cancellationToken = default)
    {
        manager.AddOrUpdate(@event.Attribute.Id, @event.Attribute.Queue.ToString().ToLowerInvariant(), () => Publish(@event.Instance, CancellationToken.None), () => @event.Attribute.CronExpression);

        return Task.CompletedTask;
    }

    [HangfireJobName("{0}: Publish recurring event")]
    public async Task Publish(IHangfireRecurringEvent recurringEvent, CancellationToken cancellationToken = default)
    {
        await mediator.Publish(recurringEvent, cancellationToken).ConfigureAwait(false);
    }
}
