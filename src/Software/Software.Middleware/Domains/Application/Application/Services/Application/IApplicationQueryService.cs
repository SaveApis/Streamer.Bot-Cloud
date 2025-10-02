using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.VO;

namespace Software.Middleware.Domains.Application.Application.Services.Application;

public interface IApplicationQueryService
{
    Task<IReadOnlyCollection<ApplicationGetDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApplicationGetDto?> GetByKeyAsync(ApplicationKey key, CancellationToken cancellationToken = default);
    Task<ApplicationGetDto?> GetByAuthIdAsync(string authId, CancellationToken cancellationToken = default);
}
