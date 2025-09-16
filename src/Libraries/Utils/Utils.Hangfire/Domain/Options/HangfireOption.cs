namespace Utils.Hangfire.Domain.Options;

public class HangfireOption
{
    public required int WorkerCount { get; init; }
    public required HangfireRedisOption Redis { get; init; }
    public required HangfireDashboardOption Dashboard { get; init; }
}
