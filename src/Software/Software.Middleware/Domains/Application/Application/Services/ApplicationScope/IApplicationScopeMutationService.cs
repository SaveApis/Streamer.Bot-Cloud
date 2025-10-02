using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.VO;

namespace Software.Middleware.Domains.Application.Application.Services.ApplicationScope;

public interface IApplicationScopeMutationService
{
    Task<ApplicationScopeKey?> CreateAsync(ApplicationScopeCreateDto dto, CancellationToken cancellationToken = default);
    Task<ApplicationScopeKey?> UpdateAsync(ApplicationScopeKey key, ApplicationScopeUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(ApplicationScopeKey key, CancellationToken cancellationToken = default);
}
