using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Application.Events;

public class RemovableRecurringEventFoundEvent(string id) : IHangfireEvent
{
    public string Id { get; } = id;

    public override string ToString()
    {
        return Id;
    }
}
