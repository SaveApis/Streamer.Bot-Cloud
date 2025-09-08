using System.Reflection;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Throttling;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Dtos;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("hangfire:recurring-events:synchronize")]
public class SynchronizeRecurringEventsJob(ILogger<IJob<ApplicationStartedEvent>> logger, IMediator mediator, IRecurringJobManagerV2 manager, IEnumerable<IRecurringHangfireEvent> recurringEvents)
    : BaseJob<ApplicationStartedEvent>(logger)
{
    protected override bool CheckSupport(ApplicationStartedEvent @event)
    {
        return @event.ApplicationType == ApplicationType.Server;
    }

    [HangfireJobName("Synchronize recurring events")]
    public override async Task RunAsync(ApplicationStartedEvent @event, CancellationToken cancellationToken = default)
    {
        var codeRecurringEvents = recurringEvents.ToDictionary(recurringEvent => recurringEvent,
            recurringEvent => recurringEvent.GetType().GetCustomAttribute<HangfireRecurringEventAttribute>()
                              ?? throw new InvalidOperationException($"Recurring event must have {nameof(HangfireRecurringEventAttribute)}"));
        var hangfireRecurringEvents = manager.Storage.GetConnection().GetRecurringJobs() ?? [];

        var summaries = new List<RecurringEventSummaryDto>();
        foreach (var pair in codeRecurringEvents)
        {
            var summary = new RecurringEventSummaryDto(pair.Value.Id);
            summary.EnrichCode(pair);
            summaries.Add(summary);
        }

        foreach (var dto in hangfireRecurringEvents)
        {
            var summary = summaries.FirstOrDefault(s => s.Id == dto.Id);
            if (summary is null)
            {
                summary = new RecurringEventSummaryDto(dto.Id);
                summaries.Add(summary);
            }
            summary.EnrichHangfire(dto);
        }

        var events = summaries.ConvertAll(dto => new RecurringEventFoundEvent(dto));
        var tasks = events.ConvertAll(e => mediator.Publish(e, cancellationToken));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
