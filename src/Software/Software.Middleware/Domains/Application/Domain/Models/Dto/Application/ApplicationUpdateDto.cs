namespace Software.Middleware.Domains.Application.Domain.Models.Dto.Application;

public class ApplicationUpdateDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string AuthId { get; init; }
    public required string AuthSecret { get; init; }
    public required IReadOnlyList<string> Scopes { get; init; }
}
