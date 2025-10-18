namespace Utils.Rest.Domain.Models.Dto;

public class ErrorDto
{
    public required string ErrorCode { get; init; }
    public required string ErrorMessage { get; init; }
}
