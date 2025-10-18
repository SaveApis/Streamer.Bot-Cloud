using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Utils.Rest.Application.Backend.REST.Middleware;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        var path = context.Request.Path;
        var fullPath = context.Request.Path + context.Request.QueryString;
        var url = $"{context.Request.Scheme}://{context.Request.Host}{fullPath}";
        var method = context.Request.Method;

        var requestQueries = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var requestHeaders = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
        var requestBody = await ReadRequestBodyAsync(context.Request);

        var requestData = new Dictionary<string, object>
        {
            ["path"] = path,
            ["fullPath"] = fullPath,
            ["url"] = url,
            ["method"] = method,
            ["requestQueries"] = requestQueries,
            ["requestHeaders"] = requestHeaders,
            ["requestBody"] = requestBody,
        };
        using (logger.BeginScope(requestData))
        {
            logger.LogDebug("Processing HTTP {Method} {Path}", method, fullPath);
        }

        await next(context);

        var statusCode = context.Response.StatusCode;
        var responseHeaders = context.Response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        var responseData = new Dictionary<string, object>
        {
            ["statusCode"] = statusCode, ["responseHeaders"] = responseHeaders,
        };
        var timeData = new Dictionary<string, object>
        {
            ["startTime"] = startTime, ["endTime"] = endTime, ["duration"] = duration.TotalMilliseconds,
        };

        var responseLogData = requestData
            .Concat(responseData)
            .Concat(timeData)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        using (logger.BeginScope(responseLogData))
        {
            logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {Duration}ms", method, fullPath, statusCode, duration.TotalMilliseconds);
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }
}
