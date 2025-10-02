using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Application.Mapping;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Services.ApplicationScope;

public class ApplicationScopeQueryService(ILogger logger, IDbContextFactory<CoreReadDbContext> factory, IApplicationMapper mapper) : IApplicationScopeQueryService
{
    public async Task<IReadOnlyList<ApplicationScopeGetDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching all application scopes from database");
        var scopes = await context.ApplicationScopes
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        logger.Information("Fetched {Count} application scopes from database", scopes.Count);

        return scopes.Select(mapper.ToDto).ToList();
    }
    public async Task<ApplicationScopeGetDto?> GetByKeyAsync(ApplicationScopeKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application scope with key '{Key}'", key);
        var scope = await context.ApplicationScopes
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (scope == null)
        {
            logger.Warning("No application scope found with key '{Key}'", key);
            return null;
        }

        logger.Information("Fetched application scope with key '{Key}'", key);
        return mapper.ToDto(scope);
    }
}
