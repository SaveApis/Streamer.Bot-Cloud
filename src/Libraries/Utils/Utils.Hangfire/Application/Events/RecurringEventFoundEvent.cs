using Utils.Hangfire.Domain.Dtos;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Application.Events;

public class RecurringEventFoundEvent(RecurringEventSummaryDto summary) : IHangfireEvent
{
    public RecurringEventSummaryDto Summary { get; } = summary;

    public override string ToString()
    {
        return Summary.Id;
    }
}
