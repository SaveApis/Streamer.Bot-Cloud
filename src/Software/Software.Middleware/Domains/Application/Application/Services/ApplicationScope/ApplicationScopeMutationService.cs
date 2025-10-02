using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Services.ApplicationScope;

public class ApplicationScopeMutationService(ILogger logger, IDbContextFactory<CoreWriteDbContext> factory) : IApplicationScopeMutationService
{
    public async Task<ApplicationScopeKey?> CreateAsync(ApplicationScopeCreateDto dto, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Checking for existing application scope with key '{Key}'", dto.Key);
        if (await context.ApplicationScopes.AnyAsync(x => x.Key == dto.Key, cancellationToken))
        {
            logger.Warning("Application scope with key '{Key}' already exists", dto.Key);
            return null;
        }

        var applicationScopeKey = ApplicationScopeKey.From(dto.Key);
        logger.Verbose("Creating new application scope with key '{Key}'", applicationScopeKey);

        var applicationScope = ApplicationScopeEntity.Create(applicationScopeKey, dto.Name, dto.Description);
        await context.ApplicationScopes.AddAsync(applicationScope, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.Information("Application scope with key '{Key}' created successfully", applicationScopeKey);

        return applicationScopeKey;
    }

    public async Task<ApplicationScopeKey?> UpdateAsync(ApplicationScopeKey key, ApplicationScopeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application scope with key '{Key}' for update", key);
        var existingApplicationScope = await context.ApplicationScopes
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken);
        if (existingApplicationScope == null)
        {
            logger.Warning("Application scope with key '{Key}' not found for update", key);
            return null;
        }

        logger.Verbose("Checking for changes in application scope with key '{Key}'", key);
        if (!existingApplicationScope.HasChanges(dto.Name, dto.Description))
        {
            logger.Information("No changes detected for application scope with key '{Key}'. Skipping update", key);
            return key;
        }

        logger.Verbose("Updating application scope with key '{Key}'", key);
        existingApplicationScope.UpdateName(dto.Name);
        existingApplicationScope.UpdateDescription(dto.Description);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.Information("Application scope with key '{Key}' updated successfully", key);

        return key;
    }

    public async Task<bool> DeleteAsync(ApplicationScopeKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application scope with key '{Key}' for deletion", key);
        var existingApplicationScope = await context.ApplicationScopes
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken);
        if (existingApplicationScope == null)
        {
            logger.Warning("Application scope with key '{Key}' not found for deletion", key);
            return false;
        }

        logger.Verbose("Deleting application scope with key '{Key}'", key);
        context.ApplicationScopes.Remove(existingApplicationScope);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.Information("Application scope with key '{Key}' deleted successfully", key);

        return true;
    }
}
