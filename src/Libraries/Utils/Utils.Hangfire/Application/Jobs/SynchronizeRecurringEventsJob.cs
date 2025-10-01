using System.Reflection;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Throttling;
using MediatR;
using Serilog;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;
using IRecurringJobManager = Hangfire.IRecurringJobManager;
using JobStorage = Hangfire.JobStorage;

namespace Utils.Hangfire.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("core:hangfire:recurring-events:synchronize")]
public class SynchronizeRecurringEventsJob(ILogger logger, IRecurringJobManager manager, IMediator mediator, IEnumerable<IHangfireRecurringEvent> recurringEvents) : BaseHangfireJob<ApplicationStartedEvent>(logger)
{
    private RecurringJobOptions Options { get; } = new()
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")
    };

    [HangfireJobName("Synchronize recurring events")]
    public override Task RunAsync(ApplicationStartedEvent @event, CancellationToken cancellationToken = default)
    {
        var codeRecurringJobs = recurringEvents.ToList();
        var codeAttributes = codeRecurringJobs
            .Where(x => x.GetType().GetCustomAttribute<HangfireRecurringEventAttribute>() != null)
            .ToDictionary(x => x.GetType().GetCustomAttribute<HangfireRecurringEventAttribute>()!.Id,
                x => new Tuple<IHangfireRecurringEvent, HangfireRecurringEventAttribute>(x, x.GetType().GetCustomAttribute<HangfireRecurringEventAttribute>()!));

        var hangfireRecurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs() ?? [];

        var codeIds = codeAttributes.Keys.ToHashSet();
        var hangfireIds = hangfireRecurringJobs.Select(x => x.Id).ToHashSet();

        var toAdd = codeIds.Except(hangfireIds).ToList();
        var toRemove = hangfireIds.Except(codeIds).ToList();
        var toUpdate = codeIds.Intersect(hangfireIds).ToList();
        Logger.Information("Found {AddCount} recurring events to add, {RemoveCount} recurring events to remove and {UpdateCount} recurring events to update", toAdd.Count, toRemove.Count, toUpdate.Count);

        Add(toAdd, codeAttributes);
        Remove(toRemove);
        Update(toUpdate, codeAttributes);

        return Task.CompletedTask;
    }

    private void Add(List<string> toAdd, Dictionary<string, Tuple<IHangfireRecurringEvent, HangfireRecurringEventAttribute>> codeAttributes)
    {
        foreach (var id in toAdd)
        {
            var (instance, info) = codeAttributes[id];

            manager.AddOrUpdate(info.Id, info.Queue.ToString().ToLowerInvariant(), () => PublishEvent(instance, CancellationToken.None), () => info.CronExpression, Options);
        }
    }

    private void Remove(List<string> toRemove)
    {
        foreach (var id in toRemove)
        {
            manager.RemoveIfExists(id);
        }
    }

    private void Update(List<string> toUpdate, Dictionary<string, Tuple<IHangfireRecurringEvent, HangfireRecurringEventAttribute>> codeAttributes)
    {
        foreach (var id in toUpdate)
        {
            var (instance, info) = codeAttributes[id];

            manager.AddOrUpdate(info.Id, info.Queue.ToString().ToLowerInvariant(), () => PublishEvent(instance, CancellationToken.None), () => info.CronExpression, Options);
        }
    }

    [HangfireJobName("{0}: Publish recurring event")]
    public async Task PublishEvent(IHangfireRecurringEvent @event, CancellationToken cancellationToken = default)
    {
        await mediator.Publish(@event, cancellationToken).ConfigureAwait(false);
    }
}
