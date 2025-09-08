using Microsoft.Extensions.Configuration;
using Utils.Hangfire.Domain.Types;

namespace Utils.Hangfire.Application.Extensions;

public static class ConfigurationExtensions
{
    public static ApplicationType GetHangfireApplicationType(this IConfiguration configuration)
    {
        var applicationTypeString = configuration["application_type"];
        if (string.IsNullOrWhiteSpace(applicationTypeString))
        {
            return ApplicationType.Unknown;
        }

        return Enum.TryParse<ApplicationType>(applicationTypeString, true, out var applicationType)
            ? applicationType
            : ApplicationType.Unknown;
    }
}
