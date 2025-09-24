namespace Utils.Jwt.Infrastructure;

public interface IJwtBuilder
{
    IJwtBuilder OverwriteExpirationTime(TimeSpan timeSpan);
    IJwtBuilder WithClaim(string type, string value);

    string Build();
}
