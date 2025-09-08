namespace Utils.Hangfire.Domain.Options;

public class DashboardOption
{
    public string? PrefixPath { get; init; }
    public required string HangfireDashboardTitle { get; init; }

    public required int MaxSucceededListLength { get; init; }
    public required int MaxDeletedListLength { get; init; }
    public required int MaxStateHistoryLength { get; init; }
}
