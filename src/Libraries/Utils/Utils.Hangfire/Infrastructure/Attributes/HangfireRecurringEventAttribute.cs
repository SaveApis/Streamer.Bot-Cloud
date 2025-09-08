using Utils.Hangfire.Domain.Types;

namespace Utils.Hangfire.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HangfireRecurringEventAttribute(string id, string cron, HangfireQueue queue) : Attribute
{
    public string Id { get; } = id;
    public string Cron { get; } = cron;
    public HangfireQueue Queue { get; } = queue;
}
