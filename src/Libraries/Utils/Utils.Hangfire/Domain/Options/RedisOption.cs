namespace Utils.Hangfire.Domain.Options;

public class RedisOption
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public required int Database { get; init; }

    public required bool UseSsl { get; init; }

    public required string Prefix { get; init; }
}
