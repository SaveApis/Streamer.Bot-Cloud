using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.Encryption.Infrastructure.Services.Encryption;

namespace Software.Middleware.Domains.Application.Application.Backend.Authorization;

public class ApplicationBasicAuthHandler(
    IOptionsMonitor<ApplicationBasicAuthHandlerOption> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IDbContextFactory<CoreReadDbContext> factory,
    IEncryptionService encryptionService) : AuthenticationHandler<ApplicationBasicAuthHandlerOption>(options, logger, encoder)
{
    public const string SchemeName = "ApplicationBasic";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationValues = Context.Request.Headers.Authorization;
        if (!AuthenticationHeaderValue.TryParse(authorizationValues, out var headerValue) || !string.Equals(SchemeName, headerValue.Scheme))
        {
            return AuthenticateResult.NoResult();
        }

        var parameter = headerValue.Parameter;
        if (string.IsNullOrWhiteSpace(parameter))
        {
            return AuthenticateResult.Fail("Invalid Authorization Parameter");
        }

        var credentialBytes = Convert.FromBase64String(parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        if (credentials.Length != 2)
        {
            return AuthenticateResult.Fail("Invalid Authorization Credential");
        }

        var authId = credentials[0];
        var authSecret = credentials[1];

        await using var context = await factory.CreateDbContextAsync();
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.AuthId == authId)
            .ConfigureAwait(false);
        if (application == null)
        {
            return AuthenticateResult.Fail("Invalid Application");
        }

        if (!application.VerifySecret(authSecret, encryptionService))
        {
            return AuthenticateResult.Fail("Invalid Application Secret");
        }

        Logger.LogInformation("Successfully authenticated application: {key}", application.Key);

        var scopeClaims = application.Scopes.Select(scope => new Claim(ClaimTypes.Role, scope.Key)).ToList();
        var identity = new ClaimsIdentity(scopeClaims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return AuthenticateResult.Success(ticket);
    }
}
