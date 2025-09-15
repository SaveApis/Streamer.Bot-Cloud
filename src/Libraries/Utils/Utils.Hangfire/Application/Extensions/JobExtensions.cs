using System.Reflection;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire.Application.Extensions;

public static class JobExtensions
{
    public static HangfireQueue GetQueue<TEvent>(this IHangfireJob<TEvent> job) where TEvent : IHangfireEvent
    {
        var attr = job.GetType().GetCustomAttribute<HangfireQueueAttribute>();

        return attr?.Queue ?? HangfireQueue.Default;
    }
}
