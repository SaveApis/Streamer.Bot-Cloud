using StackExchange.Redis;

namespace Utils.Hangfire.Domain.Options;

public class HangfireOption
{
    public required RedisOption Redis { get; init; }
    public required DashboardOption Dashboard { get; init; }

    public required int WorkerCount { get; init; }
    public required TimeSpan ExpirationTimeout { get; init; }

    public override string ToString()
    {
        var options = new ConfigurationOptions
        {
            AllowAdmin = true,
            AbortOnConnectFail = true,
            DefaultDatabase = Redis.Database,
            Password = Redis.Password,
            Ssl = Redis.UseSsl,
            User = Redis.UserName,
        };
        options.EndPoints.Add(Redis.Host, Redis.Port);

        return options.ToString();
    }
}
