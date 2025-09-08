using Utils.Hangfire.Infrastructure.Handlers.Authorization;

namespace Software.Middleware.Application.Hangfire.Handlers.Authorization;

public class TemporaryAuthenticationHandler : IDashboardAuthorizationHandler
{
    public Task<bool> AuthorizeAsync(string identifier, string password, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
