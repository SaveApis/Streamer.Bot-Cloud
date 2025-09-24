using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Utils.Jwt.Domain.Options;
using Utils.Jwt.Infrastructure;

namespace Utils.Jwt.Application.Builders;

public class JwtBuilder(IOptionsMonitor<JwtOption> monitor) : IJwtBuilder
{
    private TimeSpan? _customExpiryTimeSpan;
    private readonly List<Claim> _claims = [];

    private JwtOption Options => monitor.CurrentValue;

    public IJwtBuilder OverwriteExpirationTime(TimeSpan timeSpan)
    {
        _customExpiryTimeSpan = timeSpan;

        return this;
    }

    public IJwtBuilder WithClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));

        return this;
    }

    public string Build()
    {
        var now = DateTime.UtcNow;
        var handler = new JwtSecurityTokenHandler();

        var signingKeyBytes = Convert.FromBase64String(Options.SigningKey);

        var descriptor = new SecurityTokenDescriptor
        {
            Audience = Options.Audience,
            Issuer = Options.Issuer,
            Subject = new ClaimsIdentity(_claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = _customExpiryTimeSpan == null ? now.AddMinutes(Options.ExpirationMinutes) : now.Add(_customExpiryTimeSpan.Value),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKeyBytes), SecurityAlgorithms.HmacSha512),
            TokenType = "JWT",
        };

        var token = handler.CreateToken(descriptor);

        return handler.WriteToken(token);
    }
}
