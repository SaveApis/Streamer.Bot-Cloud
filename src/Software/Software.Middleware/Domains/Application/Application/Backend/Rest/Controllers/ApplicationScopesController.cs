using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Software.Middleware.Domains.Application.Application.Services.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Common.Application.Constants;

namespace Software.Middleware.Domains.Application.Application.Backend.Rest.Controllers;

[ApiController]
[Route("applications/scopes")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.ApplicationBasic)]
public class ApplicationScopesController(IApplicationScopeQueryService queryService) : ControllerBase
{
    [HttpGet("all")]
    [Authorize(Roles = ApplicationScopeKeys.Application.Scope.Read)]
    public async Task<ActionResult<IReadOnlyList<ApplicationScopeGetDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var scopes = await queryService.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return Ok(scopes);
    }

    [HttpGet]
    [Authorize(Roles = ApplicationScopeKeys.Application.Scope.Read)]
    public async Task<ActionResult<ApplicationScopeGetDto>> GetByKey([FromQuery] string key, CancellationToken cancellationToken = default)
    {
        var scope = await queryService.GetByKeyAsync(ApplicationScopeKey.From(key), cancellationToken).ConfigureAwait(false);
        if (scope is null)
        {
            return NotFound();
        }

        return Ok(scope);
    }
}
