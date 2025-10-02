using Vogen;

namespace Software.Middleware.Domains.Application.Domain.VO;

[ValueObject<string>]
public partial class ApplicationScopeKey
{
    private static Validation Validate(string input)
    {
        return string.IsNullOrWhiteSpace(input)
             ? Validation.Invalid("Application scope key cannot be null or empty")
            : Validation.Ok;
    }
}
