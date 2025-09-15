using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Application.Events;

public class CreatableRecurringEventFoundEvent(IHangfireRecurringEvent instance, HangfireRecurringEventAttribute attribute) : IHangfireEvent
{
    public IHangfireRecurringEvent Instance { get; } = instance;
    public HangfireRecurringEventAttribute Attribute { get; } = attribute;

    public override string ToString()
    {
        return Attribute.Id;
    }
}
