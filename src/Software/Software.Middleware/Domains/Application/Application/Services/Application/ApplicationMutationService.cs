using System.Text;
using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Domain.Types;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.Encryption.Infrastructure.Services.Encryption;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Services.Application;

public class ApplicationMutationService(ILogger logger, IDbContextFactory<CoreWriteDbContext> factory, IEncryptionService encryptionService) : IApplicationMutationService
{
    public async Task<ApplicationKey?> CreateAsync(ApplicationCreateDto dto, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Checking for existing application with key '{Key}'", dto.Key);
        if (await context.Applications.AnyAsync(x => x.Key == dto.Key, cancellationToken).ConfigureAwait(false))
        {
            logger.Warning("Application with key '{Key}' already exists", dto.Key);
            return null;
        }

        var applicationKey = ApplicationKey.From(dto.Key);
        logger.Verbose("Creating new application with key '{Key}'", applicationKey);

        logger.Verbose("Fetching selected scopes ({Scopes}) for application '{Key}'", string.Join(", ", dto.Scopes), applicationKey);
        var scopes = await context.ApplicationScopes
            .Where(x => dto.Scopes.Contains(x.Key))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(dto.AuthSecret));
        var encryptedAuthSecret = encryptionService.Encrypt(normalSecret, null, out var iv);

        var application = ApplicationEntity.Create(applicationKey, dto.Name, dto.Description, dto.AuthId, encryptedAuthSecret, iv, scopes);
        await context.Applications.AddAsync(application, cancellationToken).ConfigureAwait(false);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.Information("Application with key '{Key}' created successfully", applicationKey);

        return applicationKey;
    }

    public async Task<ApplicationKey?> UpdateAsync(ApplicationKey key, ApplicationUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with key '{Key}' for update", key);
        var existingApplication = await context.Applications
            .Include(x => x.Scopes)
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (existingApplication == null)
        {
            logger.Warning("Application with key '{Key}' not found for update", key);
            return null;
        }

        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(dto.AuthSecret));
        var encryptedAuthSecret = encryptionService.Encrypt(normalSecret, existingApplication.Iv, out var iv);

        logger.Verbose("Checking for changes in application with key '{Key}'", key);
        if (!existingApplication.HasChanges(dto.Name, dto.Description, dto.AuthId, encryptedAuthSecret, dto.Scopes))
        {
            logger.Information("No changes detected for application with key '{Key}'. Skipping update", key);
            return key;
        }

        logger.Verbose("Fetching selected scopes ({Scopes}) for application '{Key}'", string.Join(", ", dto.Scopes), key);
        var scopes = await context.ApplicationScopes
            .Where(x => dto.Scopes.Contains(x.Key))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        logger.Verbose("Updating application with key '{Key}'", key);
        existingApplication.UpdateName(dto.Name);
        existingApplication.UpdateDescription(dto.Description);
        existingApplication.UpdateCredentials(dto.AuthId, encryptedAuthSecret, iv);
        existingApplication.UpdateScopes(scopes);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.Information("Application with key '{Key}' updated successfully", key);
        return key;
    }

    public async Task<bool> DeleteAsync(ApplicationKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with key '{Key}' for deletion", key);
        var application = await context.Applications
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key '{Key}' not found for deletion", key);
            return false;
        }

        logger.Verbose("Deleting application with key '{Key}'", key);
        context.Applications.Remove(application);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.Information("Application with key '{Key}' deleted successfully", key);
        return true;
    }

    public async Task<ApplicationKey?> ActivateAsync(ApplicationKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with key '{Key}' for activation", key);
        var application = await context.Applications
            .SingleOrDefaultAsync(a => a.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key '{Key}' not found for activation", key);
            return null;
        }

        if (application.State == ApplicationState.Active)
        {
            logger.Information("Application with key '{Key}' is already active", key);
            return key;
        }

        logger.Verbose("Activating application with key '{Key}'", key);
        application.Activate();

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.Information("Application with key '{Key}' activated successfully", key);

        return key;
    }

    public async Task<ApplicationKey?> DeactivateAsync(ApplicationKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with key '{Key}' for deactivation", key);
        var application = await context.Applications
            .SingleOrDefaultAsync(a => a.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key '{Key}' not found for deactivation", key);
            return null;
        }

        if (application.State == ApplicationState.Inactive)
        {
            logger.Information("Application with key '{Key}' is already inactive", key);
            return key;
        }

        logger.Verbose("Deactivating application with key '{Key}'", key);
        application.Deactivate();

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.Information("Application with key '{Key}' deactivated successfully", key);

        return key;
    }
}
