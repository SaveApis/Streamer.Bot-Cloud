using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Utils.Hangfire.Infrastructure.Handlers.Authorization;

namespace Utils.Hangfire.Application.Handler.Authorization;

public class DefaultDashboardAuthorizationFilter(IDashboardAuthorizationHandler handler) : IDashboardAsyncAuthorizationFilter
{
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var authorizationHeader = httpContext.Request.Headers.Authorization;
        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationHeaderValue))
        {
            Challenge(httpContext);
            return false;
        }

        var parameter = authorizationHeaderValue.Parameter ?? string.Empty;
        if (string.IsNullOrWhiteSpace(parameter))
        {
            Challenge(httpContext);
            return false;
        }

        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
        var parts = credentials.Split(':', 2);
        if (parts.Length != 2)
        {
            Challenge(httpContext);
            return false;
        }

        var identifier = UrlEncoder.Default.Encode(parts[0]);
        var password = UrlEncoder.Default.Encode(parts[1]);

        var isAuthorized = await handler.AuthorizeAsync(identifier, password, httpContext.RequestAborted);
        if (isAuthorized)
        {
            return true;
        }

        Challenge(httpContext);
        return false;
    }

    private static void Challenge(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}
