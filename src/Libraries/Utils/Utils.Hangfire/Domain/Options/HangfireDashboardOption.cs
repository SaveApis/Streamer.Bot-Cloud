namespace Utils.Hangfire.Domain.Options;

public class HangfireDashboardOption
{
    public required string AppPath { get; init; }
    public string? PrefixPath { get; init; }
    public required string DashboardTitle { get; init; }
}
