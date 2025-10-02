using Vogen;

namespace Software.Middleware.Domains.Application.Domain.VO;

[ValueObject<string>]
public partial class ApplicationKey
{
    private static Validation Validate(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? Validation.Invalid("Application key cannot be null or empty")
            : Validation.Ok;
    }
}
