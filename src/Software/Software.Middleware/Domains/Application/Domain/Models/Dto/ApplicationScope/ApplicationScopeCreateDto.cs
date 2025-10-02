namespace Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;

public class ApplicationScopeCreateDto
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
