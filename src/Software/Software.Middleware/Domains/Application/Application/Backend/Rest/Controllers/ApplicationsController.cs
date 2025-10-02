using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Software.Middleware.Domains.Application.Application.Services.Application;
using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Application.Constants;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Backend.Rest.Controllers;

[ApiController]
[Route("applications")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.ApplicationBasic)]
public class ApplicationsController(ILogger logger, IApplicationQueryService queryService, IApplicationMutationService mutationService) : ControllerBase
{
    [HttpGet("all")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<ActionResult<IEnumerable<ApplicationGetDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var applications = await queryService.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return Ok(applications);
    }

    [HttpGet]
    [Authorize(Roles = ApplicationScopeKeys.Application.Read)]
    public async Task<ActionResult<ApplicationGetDto>> GetByKey([FromQuery] string key, CancellationToken cancellationToken = default)
    {
        var application = await queryService.GetByKeyAsync(ApplicationKey.From(key), cancellationToken).ConfigureAwait(false);
        if (application is null)
        {
            return NotFound();
        }

        return Ok(application);
    }

    [HttpPost("create")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Create)]
    public async Task<ActionResult<ApplicationGetDto>> Create([FromBody] ApplicationCreateDto dto, CancellationToken cancellationToken = default)
    {
        var createdKey = await mutationService.CreateAsync(dto, cancellationToken).ConfigureAwait(false);
        if (createdKey is null)
        {
            return Conflict("Application with the same key already exists");
        }

        var createdApplication = await queryService.GetByKeyAsync(createdKey, cancellationToken).ConfigureAwait(false);
        if (createdApplication == null)
        {
            logger.Error("Created application with key {ApplicationKey} could not be retrieved", createdKey);
            return StatusCode(500, "Created application could not be retrieved");
        }

        return CreatedAtAction(nameof(GetByKey), new
        {
            key = createdApplication.Key
        }, createdApplication);
    }

    [HttpPut("update")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<ActionResult<ApplicationGetDto>> Update([FromQuery] string key, [FromBody] ApplicationUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var updatedKey = await mutationService.UpdateAsync(ApplicationKey.From(key), dto, cancellationToken).ConfigureAwait(false);
        if (updatedKey is null)
        {
            return NotFound();
        }

        var updatedApplication = await queryService.GetByKeyAsync(updatedKey, cancellationToken).ConfigureAwait(false);
        if (updatedApplication == null)
        {
            logger.Error("Updated application with key {ApplicationKey} could not be retrieved", updatedKey);
            return StatusCode(500, "Updated application could not be retrieved");
        }

        return Ok(updatedApplication);
    }

    [HttpPatch("activate")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<IActionResult> Activate([FromQuery] string key, CancellationToken cancellationToken = default)
    {
        var activatedKey = await mutationService.ActivateAsync(ApplicationKey.From(key), cancellationToken).ConfigureAwait(false);
        if (activatedKey is null)
        {
            return NotFound();
        }

        var activatedApplication = await queryService.GetByKeyAsync(activatedKey, cancellationToken).ConfigureAwait(false);
        if (activatedApplication == null)
        {
            logger.Error("Activated application with key {ApplicationKey} could not be retrieved", activatedKey);
            return StatusCode(500, "Activated application could not be retrieved");
        }

        return Ok(activatedApplication);
    }

    [HttpPatch("deactivate")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Update)]
    public async Task<IActionResult> Deactivate([FromQuery] string key, CancellationToken cancellationToken = default)
    {
        var deactivatedKey = await mutationService.DeactivateAsync(ApplicationKey.From(key), cancellationToken).ConfigureAwait(false);
        if (deactivatedKey is null)
        {
            return NotFound();
        }

        var deactivatedApplication = await queryService.GetByKeyAsync(deactivatedKey, cancellationToken).ConfigureAwait(false);
        if (deactivatedApplication == null)
        {
            logger.Error("Deactivated application with key {ApplicationKey} could not be retrieved", deactivatedKey);
            return StatusCode(500, "Deactivated application could not be retrieved");
        }

        return Ok(deactivatedApplication);
    }

    [HttpDelete("delete")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Delete)]
    public async Task<IActionResult> Delete([FromQuery] string key, CancellationToken cancellationToken = default)
    {
        var deleted = await mutationService.DeleteAsync(ApplicationKey.From(key), cancellationToken).ConfigureAwait(false);

        return deleted
            ? NoContent()
            : NotFound();
    }
}
