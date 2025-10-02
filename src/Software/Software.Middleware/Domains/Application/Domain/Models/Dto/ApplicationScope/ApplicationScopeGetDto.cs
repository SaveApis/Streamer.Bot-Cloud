namespace Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;

public class ApplicationScopeGetDto
{
    public required string Key { get; init; }

    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public required string Name { get; init; }
    public string? Description { get; init; }
}
