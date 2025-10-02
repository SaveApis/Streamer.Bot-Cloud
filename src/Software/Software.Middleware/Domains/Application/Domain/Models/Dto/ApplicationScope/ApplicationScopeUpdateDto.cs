namespace Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;

public class ApplicationScopeUpdateDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
