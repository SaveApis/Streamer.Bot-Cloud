using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.VO;

namespace Software.Middleware.Domains.Application.Application.Services.ApplicationScope;

public interface IApplicationScopeQueryService
{
    Task<IReadOnlyList<ApplicationScopeGetDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApplicationScopeGetDto?> GetByKeyAsync(ApplicationScopeKey key, CancellationToken cancellationToken = default);
}
