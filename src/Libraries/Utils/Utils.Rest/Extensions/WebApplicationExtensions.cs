using Microsoft.AspNetCore.Builder;
using Utils.Rest.Application.Backend.REST.Middleware;

namespace Utils.Rest.Extensions;

public static class WebApplicationExtensions
{
    public static void UseLoggingMiddleware(this WebApplication application)
    {
        application.UseMiddleware<LoggingMiddleware>();
    }

    public static void UseExceptionMiddleware(this WebApplication application)
    {
        application.UseMiddleware<ExceptionMiddleware>();
    }
}
