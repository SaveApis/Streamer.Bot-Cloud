using System.Reflection;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Throttling;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("core:recurring-events:synchronize")]
public class CollectRecurringEventsJob(ILogger<IHangfireJob<ApplicationStartedEvent>> logger, IEnumerable<IHangfireRecurringEvent> codeRecurringEvents, IMediator mediator) : BaseHangfireJob<ApplicationStartedEvent>(logger)
{
    [HangfireJobName("Collect recurring events job")]
    public override async Task RunAsync(ApplicationStartedEvent @event, CancellationToken cancellationToken = default)
    {
        var codeEvents = ReadCodeEvents();
        var hangfireEvents = ReadHangfireEvents();

        var toAdd = codeEvents.Keys.Except(hangfireEvents.Keys).ToList();
        var toRemove = hangfireEvents.Keys.Except(codeEvents.Keys).ToList();
        var toUpdate = codeEvents.Keys.Intersect(hangfireEvents.Keys).ToList();

        var toAddEvents = codeEvents
            .Where(x => toAdd.Contains(x.Key))
            .Select(x => new CreatableRecurringEventFoundEvent(x.Value.Item1, x.Value.Item2))
            .ToList();
        var toRemoveEvents = toRemove
            .Select(x => new RemovableRecurringEventFoundEvent(x))
            .ToList();
        var toUpdateEvents = codeEvents
            .Where(x => toUpdate.Contains(x.Key))
            .Select(x => new UpdatableRecurringEventFoundEvent(x.Value.Item1, x.Value.Item2))
            .ToList();

        Logger.LogInformation("Found {ToAddCount} events to add, {ToRemoveCount} events to remove, {ToUpdateCount} events to update", toAdd.Count, toRemove.Count, toUpdateEvents.Count);
        var allEvents = toAddEvents.Cast<IHangfireEvent>()
            .Concat(toRemoveEvents)
            .Concat(toUpdateEvents)
            .ToList();
        var publishTasks = allEvents
            .Select(x => mediator.Publish(x, cancellationToken))
            .ToList();
        await Task.WhenAll(publishTasks);
    }

    private Dictionary<string, Tuple<IHangfireRecurringEvent, HangfireRecurringEventAttribute>> ReadCodeEvents()
    {
        var internalDictionary = new Dictionary<string, Tuple<IHangfireRecurringEvent, HangfireRecurringEventAttribute>>();
        foreach (var recurringEvent in codeRecurringEvents)
        {
            var attribute = recurringEvent.GetType().GetCustomAttribute<HangfireRecurringEventAttribute>();
            if (attribute == null)
            {
                Logger.LogWarning("Recurring event {RecurringEventType} is missing HangfireRecurringEventAttribute", recurringEvent.GetType().FullName);
                continue;
            }

            if (internalDictionary.TryAdd(attribute.Id, Tuple.Create(recurringEvent, attribute)))
            {
                continue;
            }

            Logger.LogWarning("Found duplicated recurring event with id {RecurringEventId}", attribute.Id);
        }

        return internalDictionary;
    }

    private Dictionary<string, RecurringJobDto> ReadHangfireEvents()
    {
        var hangfireRecurringEvents = JobStorage.Current.GetConnection().GetRecurringJobs() ?? [];

        var internalDictionary = new Dictionary<string, RecurringJobDto>();
        foreach (var job in hangfireRecurringEvents.Where(job => !internalDictionary.TryAdd(job.Id, job)))
        {
            Logger.LogWarning("Found duplicated recurring job with id {RecurringJobId}", job.Id);
        }

        return internalDictionary;
    }
}
