using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.VO;

namespace Software.Middleware.Domains.Application.Application.Services.Application;

public interface IApplicationMutationService
{
    Task<ApplicationKey?> CreateAsync(ApplicationCreateDto dto, CancellationToken cancellationToken = default);
    Task<ApplicationKey?> UpdateAsync(ApplicationKey key, ApplicationUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(ApplicationKey key, CancellationToken cancellationToken = default);

    Task<ApplicationKey?> ActivateAsync(ApplicationKey key, CancellationToken cancellationToken = default);
    Task<ApplicationKey?> DeactivateAsync(ApplicationKey key, CancellationToken cancellationToken = default);
}
