using MySqlConnector;

namespace Utils.EntityFrameworkCore.Domain.Options;

public class MySqlOption
{
    public required string Host { get; init; }
    public required uint Port { get; init; }
    public required string Database { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }

    public override string ToString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = Host,
            Port = Port,
            Database = Database,
            UserID = UserName,
            Password = Password,
            Pooling = true,
            Pipelining = true,
            UseCompression = true,
            UseAffectedRows = true,
            AllowUserVariables = true,
            BrowsableConnectionString = false,
        };
        
        return builder.ToString();
    }
}
