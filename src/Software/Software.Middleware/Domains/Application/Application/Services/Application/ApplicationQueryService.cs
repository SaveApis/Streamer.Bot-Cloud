using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Application.Mapping;
using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Services.Application;

public class ApplicationQueryService(ILogger logger, IDbContextFactory<CoreReadDbContext> factory, IApplicationMapper mapper) : IApplicationQueryService
{
    public async Task<IReadOnlyCollection<ApplicationGetDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching all applications from database");
        var applications = await context.Applications
            .Include(x => x.Scopes)
            .ToListAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        logger.Information("Fetched {Count} applications from database", applications.Count);

        return applications.Select(mapper.ToDto).ToList();
    }
    public async Task<ApplicationGetDto?> GetByKeyAsync(ApplicationKey key, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with key '{Key}'", key);
        var application = await context.Applications
            .Include(x => x.Scopes)
            .SingleOrDefaultAsync(x => x.Key == key.Value, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("No application found with key '{Key}'", key);
            return null;
        }

        logger.Information("Fetched application with key '{Key}'", key);
        return mapper.ToDto(application);
    }

    public async Task<ApplicationGetDto?> GetByAuthIdAsync(string authId, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        logger.Verbose("Fetching application with authId '{AuthId}'", authId);
        var application = await context.Applications
            .Include(x => x.Scopes)
            .SingleOrDefaultAsync(x => x.AuthId == authId, cancellationToken)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("No application found with authId '{AuthId}'", authId);
            return null;
        }

        logger.Information("Fetched application with authId '{AuthId}'", authId);
        return mapper.ToDto(application);
    }
}
