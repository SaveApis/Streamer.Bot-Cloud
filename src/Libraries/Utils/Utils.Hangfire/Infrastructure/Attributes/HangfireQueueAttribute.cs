using Utils.Hangfire.Domain.Types;

namespace Utils.Hangfire.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HangfireQueueAttribute(HangfireQueue queue) : Attribute
{
    public HangfireQueue Queue { get; } = queue;
}
