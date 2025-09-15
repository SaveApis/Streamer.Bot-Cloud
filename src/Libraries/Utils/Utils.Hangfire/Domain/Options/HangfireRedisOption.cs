using StackExchange.Redis;

namespace Utils.Hangfire.Domain.Options;

public class HangfireRedisOption
{
    public required string Host { get; init; } //TODO: Has to be set in staging + production via env variables
    public required int Port { get; init; } //TODO: Has to be set in staging + production via env variables
    public required int Database { get; init; }
    public string? UserName { get; init; } //TODO: Has to be set in staging + production via env variables
    public string? Password { get; init; } //TODO: Has to be set in staging + production via env variables

    public required bool UseSsl { get; init; }
    public required string Prefix { get; init; }

    public int MaxSucceededListLength { get; init; } = 1_0000_000;
    public int MaxDeletedListLength { get; init; } = 1_0000_000;
    public int MaxStateHistoryLength { get; init; } = 1_0000;

    public override string ToString()
    {
        var options = new ConfigurationOptions();
        options.AbortOnConnectFail = true;
        options.AllowAdmin = true;
        options.EndPoints.Add(Host, Port);
        options.IncludeDetailInExceptions = true;
        options.Password = Password;
        options.User = UserName;
        options.Ssl = UseSsl;
        options.ResolveDns = true;

        return options.ToString();
    }
}
