using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.Encryption.Infrastructure.Services.Encryption;

namespace Software.Middleware.Domains.Application.Application.Services.Application;

public class ApplicationService(IDbContextFactory<CoreReadDbContext> factory, IEncryptionService encryptionService) : IApplicationService
{
    public async Task<bool> VerifyCredentialsAsync(string authId, string authSecret, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var application = await context.Applications
            .SingleOrDefaultAsync(x => x.AuthId == authId, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            return false;
        }

        var encryptedSecret = encryptionService.Encrypt(authSecret, application.Iv, out var _);

        return application.AuthSecret == encryptedSecret;
    }
}
