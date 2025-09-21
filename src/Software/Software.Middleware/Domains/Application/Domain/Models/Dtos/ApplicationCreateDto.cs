namespace Software.Middleware.Domains.Application.Domain.Models.Dtos;

public class ApplicationCreateDto
{
    public required string Key { get; init; }
    public required string Name { get; init; }

    public required string AuthId { get; init; }
    public required string AuthSecret { get; init; }
    public IReadOnlyList<string> Scopes { get; init; } = [];
}
