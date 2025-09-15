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
public class UpdateRecurringEventJob(ILogger<IHangfireJob<UpdatableRecurringEventFoundEvent>> logger, IRecurringJobManagerV2 manager, IMediator mediator) : BaseHangfireJob<UpdatableRecurringEventFoundEvent>(logger)
{
    [HangfireJobName("{0}: Update recurring event")]
    public override Task RunAsync(UpdatableRecurringEventFoundEvent @event, CancellationToken cancellationToken = default)
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
