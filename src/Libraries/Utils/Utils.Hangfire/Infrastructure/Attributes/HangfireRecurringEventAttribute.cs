using Utils.Hangfire.Domain.Types;

namespace Utils.Hangfire.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HangfireRecurringEventAttribute(HangfireQueue queue, string id, string cronExpression) : Attribute
{
    public HangfireQueue Queue { get; } = queue;
    public string Id { get; } = id;
    public string CronExpression { get; } = cronExpression;
}
