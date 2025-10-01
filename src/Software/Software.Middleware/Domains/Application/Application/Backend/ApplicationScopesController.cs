using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Application.Backend.Authorization;
using Software.Middleware.Domains.Common.Application.Infrastructure;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Backend;

[ApiController]
[Route("applications/scopes")]
[Authorize(AuthenticationSchemes = ApplicationBasicAuthHandler.SchemeName)]
public class ApplicationScopesController(ILogger logger, IDbContextFactory<CoreReadDbContext> factory) : ControllerBase
{
    [HttpGet("all")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<IActionResult> GetAll()
    {
        await using var context = await factory.CreateDbContextAsync();
        var scopes = await context.ApplicationScopes.ToListAsync().ConfigureAwait(false);

        logger.Information("Successfully fetched {Count} application scopes.", scopes.Count);
        return Ok(scopes);
    }

    [HttpGet]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<IActionResult> GetByKey([FromQuery] string key)
    {
        await using var context = await factory.CreateDbContextAsync();

        logger.Debug("Fetching application scope with key {Key}", key);
        var scope = await context.ApplicationScopes.FirstOrDefaultAsync(s => s.Key == key).ConfigureAwait(false);
        if (scope == null)
        {
            logger.Warning("Application scope with key {Key} not found.", key);
            return NotFound();
        }

        logger.Information("Successfully fetched application scope with key {Key}.", key);
        return Ok(scope);
    }
}
