namespace Utils.Jwt.Domain.Options;

public class JwtOption
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public required int ExpirationMinutes { get; init; }
}
