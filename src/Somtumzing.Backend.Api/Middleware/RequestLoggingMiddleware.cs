using System.Diagnostics;

namespace Somtumzing.Backend.Api.Middleware;

public sealed class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    private static readonly EventId RequestCompletedEvent = new(3001, "HttpRequestCompleted");

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                RequestCompletedEvent,
                "HTTP request completed. {EventType} {HttpMethod} {Path} {StatusCode} {ElapsedMilliseconds}",
                "HttpRequestCompleted",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
