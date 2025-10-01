namespace Utils.Serilog.Domain.Options;

public class LoggingOption
{
    public required string ApplicationName { get; init; }
    public required BetterStackOption BetterStack { get; init; }
}
