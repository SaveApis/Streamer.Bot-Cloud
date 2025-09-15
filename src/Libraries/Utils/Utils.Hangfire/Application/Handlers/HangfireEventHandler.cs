using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Hangfire.Application.Extensions;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Handlers;

public class HangfireEventHandler<TEvent>(ILogger<HangfireEventHandler<TEvent>> logger, IEnumerable<IHangfireJob<TEvent>> assignedJobs, IBackgroundJobClientV2 client) : INotificationHandler<TEvent> where TEvent : IHangfireEvent
{
    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received hangfire event: {Event}", notification.GetType().Name);

        var jobs = assignedJobs.ToList();
        if (jobs.Count == 0)
        {
            logger.LogDebug("No jobs assigned for event: {Event}", notification.GetType().Name);
            return;
        }

        foreach (var job in jobs)
        {
            logger.LogTrace("Discovered job: {Job} for event: {Event}", job.GetType().Name, notification.GetType().Name);
            if (!await job.CheckSupportAsync(notification, cancellationToken).ConfigureAwait(false))
            {
                logger.LogTrace("Job: {Job} does not support event: {Event}", job.GetType().Name, notification.GetType().Name);
                continue;
            }

            var queue = job.GetQueue();
            var id = client.Enqueue(queue.ToString().ToLowerInvariant(), () => job.RunAsync(notification, CancellationToken.None));
            logger.LogInformation("Enqueued job: {Job} with id: {Id} for event: {Event} in queue: {Queue}", job.GetType().Name, id, notification.GetType().Name, queue);
        }
    }
}
