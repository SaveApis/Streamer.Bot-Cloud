using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Application.Backend.Authorization;
using Software.Middleware.Domains.Application.Domain.Models.Dtos;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Domain.Types;
using Software.Middleware.Domains.Common.Application.Infrastructure;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.Encryption.Infrastructure.Services.Encryption;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Backend;

[ApiController]
[Route("applications")]
[Authorize(AuthenticationSchemes = ApplicationBasicAuthHandler.SchemeName)]
public class ApplicationsController(
    ILogger logger,
    IDbContextFactory<CoreReadDbContext> readFactory,
    IDbContextFactory<CoreWriteDbContext> writeFactory,
    IEncryptionService encryptionService) : ControllerBase
{
    [HttpGet("all")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<IActionResult> GetAll()
    {
        await using var context = await readFactory.CreateDbContextAsync();
        var applications = await context.Applications
            .Include(a => a.Scopes)
            .ToListAsync()
            .ConfigureAwait(false);

        logger.Information("Successfully fetched {Count} applications.", applications.Count);
        return Ok(applications);
    }

    [HttpGet]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<IActionResult> GetByKey([FromQuery] string key)
    {
        await using var context = await readFactory.CreateDbContextAsync();

        logger.Debug("Fetching application");
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.Key == key)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key {Key} not found.", key);
            return NotFound();
        }

        logger.Information("Successfully fetched application with key {Key}.", key);
        return Ok(application);
    }

    [HttpPost("create")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Create)]
    public async Task<IActionResult> Create(ApplicationCreateDto dto)
    {
        await using var context = await writeFactory.CreateDbContextAsync();

        logger.Debug("Checking for existing application with key {Key}", dto.Key);
        if (await context.Applications.AnyAsync(a => a.Key == dto.Key).ConfigureAwait(false))
        {
            logger.Warning("Application with key {Key} already exists.", dto.Key);
            return Conflict("An application with the same key already exists.");
        }

        logger.Debug("Fetching application scopes for new application with key {Key}", dto.Key);
        var applicationScopes = await context.ApplicationScopes.ToListAsync().ConfigureAwait(false);
        var selectedScopes = applicationScopes.Where(s => dto.Scopes.Contains(s.Key)).ToList();

        logger.Debug("Creating new application with key {Key}", dto.Key);
        var application = ApplicationEntity.CreateFromApi(dto.Key, dto.Name, dto.AuthId, dto.AuthSecret, selectedScopes, encryptionService);

        context.Applications.Add(application);
        await context.SaveChangesAsync().ConfigureAwait(false);

        logger.Information("Successfully created application with key {Key}.", dto.Key);
        return CreatedAtAction(nameof(GetByKey), new
        {
            key = application.Key
        }, application);
    }

    [HttpPatch("update/name")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<IActionResult> UpdateName([FromQuery] string key, ApplicationNameUpdateDto dto)
    {
        await using var context = await writeFactory.CreateDbContextAsync();

        logger.Debug("Checking for existing application with key {Key}", key);
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.Key == key)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key {Key} not found.", key);
            return NotFound();
        }

        if (application.Source == ApplicationSource.Configuration)
        {
            logger.Warning("Cannot update name for application with key {Key} because it is configuration-based.", key);
            return BadRequest("Cannot update name for configuration-based applications.");
        }

        logger.Debug("Updating name for application with key {Key}", key);
        application.UpdateName(dto.Name);
        await context.SaveChangesAsync().ConfigureAwait(false);

        logger.Information("Successfully updated name for application with key {Key}.", key);
        return NoContent();
    }

    [HttpPatch("update/auth")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<IActionResult> UpdateAuth([FromQuery] string key, ApplicationAuthUpdateDto dto)
    {
        await using var context = await writeFactory.CreateDbContextAsync();

        logger.Debug("Checking for existing application with key {Key}", key);
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.Key == key)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key {Key} not found.", key);
            return NotFound();
        }

        if (application.Source == ApplicationSource.Configuration)
        {
            logger.Warning("Cannot update auth for application with key {Key} because it is configuration-based.", key);
            return BadRequest("Cannot update auth for configuration-based applications.");
        }

        logger.Debug("Updating auth for application with key {Key}", key);
        application.UpdateAuthId(dto.AuthId);
        application.UpdateAuthSecret(dto.AuthSecret, encryptionService);
        await context.SaveChangesAsync().ConfigureAwait(false);

        logger.Information("Successfully updated auth for application with key {Key}.", key);
        return NoContent();
    }

    [HttpPatch("update/scopes")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<IActionResult> UpdateScopes([FromQuery] string key, ApplicationScopesUpdateDto dto)
    {
        await using var context = await writeFactory.CreateDbContextAsync();

        logger.Debug("Checking for existing application with key {Key}", key);
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.Key == key)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key {Key} not found.", key);
            return NotFound();
        }

        if (application.Source == ApplicationSource.Configuration)
        {
            logger.Warning("Cannot update scopes for application with key {Key} because it is configuration-based.", key);
            return BadRequest("Cannot update scopes for configuration-based applications.");
        }

        logger.Debug("Fetching application scopes for application with key {Key}", key);
        var applicationScopes = await context.ApplicationScopes.ToListAsync().ConfigureAwait(false);
        var selectedScopes = applicationScopes.Where(s => dto.Scopes.Contains(s.Key)).ToList();

        logger.Debug("Updating scopes for application with key {Key}", key);
        application.UpdateScopes(selectedScopes);
        await context.SaveChangesAsync().ConfigureAwait(false);

        logger.Information("Successfully updated scopes for application with key {Key}.", key);
        return NoContent();
    }

    [HttpDelete("delete")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Delete)]
    public async Task<IActionResult> Delete([FromQuery] string key)
    {
        await using var context = await writeFactory.CreateDbContextAsync();

        logger.Debug("Checking for existing application with key {Key}", key);
        var application = await context.Applications
            .Include(a => a.Scopes)
            .FirstOrDefaultAsync(a => a.Key == key)
            .ConfigureAwait(false);
        if (application == null)
        {
            logger.Warning("Application with key {Key} not found.", key);
            return NotFound();
        }

        if (application.Source == ApplicationSource.Configuration)
        {
            logger.Warning("Cannot delete application with key {Key} because it is configuration-based.", key);
            return BadRequest("Cannot delete configuration-based applications.");
        }

        logger.Debug("Deleting application with key {Key}", key);
        context.Applications.Remove(application);
        await context.SaveChangesAsync().ConfigureAwait(false);

        logger.Information("Successfully deleted application with key {Key}.", key);
        return NoContent();
    }
}
