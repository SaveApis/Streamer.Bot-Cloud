namespace Utils.Hangfire.Infrastructure.Handlers.Authorization;

public interface IDashboardAuthorizationHandler
{
    Task<bool> AuthorizeAsync(string identifier, string password, CancellationToken cancellationToken = default);
}
