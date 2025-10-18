using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Utils.Rest.Domain.Models.Dto;
using Utils.Rest.Infrastructure;

namespace Utils.Rest.Application.Backend.REST.Middleware;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var dto = CreateDto(ex);
            await context.Response.WriteAsJsonAsync(dto).ConfigureAwait(false);
        }
    }

    private ErrorDto CreateDto(Exception ex)
    {
        if (ex is IKnownException knownException)
        {
            logger.LogTrace("Creating ErrorDto from known exception with code {ErrorCode}", knownException.ErrorCode);
            return new ErrorDto
            {
                ErrorCode = knownException.ErrorCode,
                ErrorMessage = knownException.ErrorMessage,
            };
        }

        logger.LogTrace("Creating ErrorDto from unknown exception");
        return new ErrorDto
        {
            ErrorCode = "UNEXPECTED",
            ErrorMessage = "An unexpected error occurred. Please try again later.",
        };
    }
}
