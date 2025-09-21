namespace Software.Middleware.Domains.Application.Domain.Models.Dtos;

public class ApplicationScopesUpdateDto
{
    public IReadOnlyList<string> Scopes { get; init; } = [];
}
