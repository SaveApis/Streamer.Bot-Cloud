using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Software.Middleware.Domains.Application.Application.Backend.Rest.Authentication.Options;
using Software.Middleware.Domains.Application.Application.Services.Application;
using Software.Middleware.Domains.Common.Application.Constants;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Backend.Rest.Authentication.Handlers;

public class ApplicationBasicAuthHandler(ILogger serilogLogger, IOptionsMonitor<ApplicationBasicAuthOption> options, ILoggerFactory logger, UrlEncoder encoder, IApplicationService applicationService, IApplicationQueryService queryService)
    : AuthenticationHandler<ApplicationBasicAuthOption>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeaderStringValues = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorizationHeaderStringValues)
            || !AuthenticationHeaderValue.TryParse(authorizationHeaderStringValues, out var headerValue)
            || headerValue.Scheme != AuthenticationSchemes.ApplicationBasic)
        {
            return AuthenticateResult.NoResult();
        }

        serilogLogger.Information("Start handling application basic auth");
        serilogLogger.Verbose("Reading authentication header parameters");
        var parameter = headerValue.Parameter;
        if (string.IsNullOrWhiteSpace(parameter))
        {
            serilogLogger.Warning("Failed authentication due to missing authorization header parameters");
            return AuthenticateResult.Fail("Missing Authorization Header Parameters");
        }

        var decodedParameter = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
        var parameterParts = decodedParameter.Split(':', 2);

        var authId = parameterParts[0];
        var authSecret = Encoding.UTF8.GetString(Convert.FromBase64String(parameterParts[1]));
        serilogLogger.Verbose("Verifying credentials for authId '{AuthId}'", authId);
        var isValid = await applicationService.VerifyCredentialsAsync(authId, authSecret, CancellationToken.None);
        if (!isValid)
        {
            serilogLogger.Warning("Failed authentication due to invalid credentials for authId '{AuthId}'", authId);
            return AuthenticateResult.Fail("Invalid Credentials");
        }

        var application = await queryService.GetByAuthIdAsync(authId, CancellationToken.None);
        if (application is null)
        {
            serilogLogger.Error("Failed to read application after successful credential verification for authId '{AuthId}'", authId);
            return AuthenticateResult.Fail("An unexpected error occurred. Please contact a developer!");
        }

        serilogLogger.Information("Successfully authenticated application '{ApplicationName}'", application.Name);
        var claims = application.Scopes.Select(x => new Claim(ClaimTypes.Role, x.Key));
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
