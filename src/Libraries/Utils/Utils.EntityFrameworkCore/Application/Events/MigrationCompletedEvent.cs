using Utils.Hangfire.Infrastructure.Events;

namespace Utils.EntityFrameworkCore.Application.Events;

public class MigrationCompletedEvent(Type dbContextType) : IHangfireEvent
{
    public Type DbContextType { get; } = dbContextType;
}
