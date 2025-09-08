using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Extensions;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Handler;

public class HangfireEventHandler<TEvent>(ILogger<HangfireEventHandler<TEvent>> logger, IBackgroundJobClientV2 client, IEnumerable<IJob<TEvent>> assignedJobs) : INotificationHandler<TEvent> where TEvent : IHangfireEvent
{
    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received event: {Name}", typeof(TEvent).Name);

        var jobs = assignedJobs.ToList();
        if (jobs.Count == 0)
        {
            logger.LogDebug("No jobs assigned for event: {Name}", typeof(TEvent).Name);
            return;
        }
        
        logger.LogDebug("Found {Count} jobs assigned for event: {Name}", jobs.Count, typeof(TEvent).Name);
        foreach (var job in jobs)
        {
            logger.LogDebug("Handling job: {JobName} for event: {EventName}", job.GetType().Name, typeof(TEvent).Name);

            if (!await job.CheckSupportAsync(notification, cancellationToken).ConfigureAwait(false))
            {
                logger.LogDebug("Job: {JobName} does not support event: {EventName}, skipping...", job.GetType().Name, typeof(TEvent).Name);
                continue;
            }

            var queue = job.GetQueue();
            var jobId = client.Enqueue(queue.ToString().ToLowerInvariant(), () => job.RunAsync(notification, CancellationToken.None));
            logger.LogInformation("Enqueued job: {JobName} with ID: {JobId} to queue: {Queue} for event: {EventName}", job.GetType().Name, jobId, queue, typeof(TEvent).Name);
        }
    }
}
