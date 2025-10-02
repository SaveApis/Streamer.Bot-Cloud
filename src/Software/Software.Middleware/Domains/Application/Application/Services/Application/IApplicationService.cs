namespace Software.Middleware.Domains.Application.Application.Services.Application;

public interface IApplicationService
{
    Task<bool> VerifyCredentialsAsync(string authId, string authSecret, CancellationToken cancellationToken = default);
}
