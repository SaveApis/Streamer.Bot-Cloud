namespace Software.Middleware.Domains.Application.Domain.Models.Dtos;

public class ApplicationAuthUpdateDto
{
    public required string AuthId { get; init; }
    public required string AuthSecret { get; init; }
}
