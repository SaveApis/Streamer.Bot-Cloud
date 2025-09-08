using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Events;

namespace Utils.Hangfire.Application.Events;

public class ApplicationStartedEvent(ApplicationType applicationType) : IHangfireEvent
{
    public ApplicationType ApplicationType { get; } = applicationType;
}
