using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Types;

namespace Software.Middleware.Domains.Application.Domain.Models.Dto.Application;

public class ApplicationGetDto
{
    public required string Key { get; init; }

    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public required ApplicationState State { get; init; }

    public required string Name { get; init; }
    public string? Description { get; init; }

    public required IReadOnlyCollection<ApplicationScopeGetDto> Scopes { get; init; }
}
