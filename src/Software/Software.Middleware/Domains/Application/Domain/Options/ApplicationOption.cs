namespace Software.Middleware.Domains.Application.Domain.Options;

public class ApplicationOption
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string AuthId { get; init; }
    public required string AuthSecret { get; init; }
    public IReadOnlyList<string> Scopes { get; init; } = [];
}
