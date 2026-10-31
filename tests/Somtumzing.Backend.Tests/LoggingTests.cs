using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Somtumzing.Backend.Api.Middleware;
using Somtumzing.Backend.Api.Services;

namespace Somtumzing.Backend.Tests;

public sealed class LoggingTests
{
    [Fact]
    public async Task RequestLogIncludesStructuredEventAndOmitsQueryString()
    {
        using var loggerProvider = new CapturingLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
        var middleware = new RequestLoggingMiddleware(
            loggerFactory.CreateLogger<RequestLoggingMiddleware>());
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/profile";
        context.Request.QueryString = new QueryString("?access_token=must-not-be-logged");
        context.Response.StatusCode = StatusCodes.Status201Created;

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        var entry = Assert.Single(loggerProvider.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal("HttpRequestCompleted", entry.Values["EventType"]);
        Assert.Equal("/api/v1/profile", entry.Values["Path"]);
        Assert.Equal(StatusCodes.Status201Created, entry.Values["StatusCode"]);
        Assert.DoesNotContain("must-not-be-logged", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExceptionHandlerReturnsGenericProblemWithTraceId()
    {
        using var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .AddScoped<IAuditLogWriter, ThrowingAuditLogWriter>()
            .BuildServiceProvider();
        var problemDetails = services.GetRequiredService<IProblemDetailsService>();
        using var loggerProvider = new CapturingLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
        var handler = new ApiExceptionHandler(
            loggerFactory.CreateLogger<ApiExceptionHandler>(),
            problemDetails,
            services.GetRequiredService<IServiceScopeFactory>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "trace-safe-test";

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException("internal secret detail"),
            CancellationToken.None);
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("trace-safe-test", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("internal secret detail", responseBody, StringComparison.Ordinal);
        var logEntry = loggerProvider.Entries[0];
        Assert.Equal(LogLevel.Error, logEntry.Level);
        Assert.Equal("UnhandledException", logEntry.Values["EventType"]);
        Assert.Equal("trace-safe-test", logEntry.Values["RequestId"]);
        Assert.Contains(loggerProvider.Entries, entry =>
            entry.Values.TryGetValue("EventType", out var eventType)
            && Equals(eventType, "AuditPersistenceFailure"));
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public List<CapturedLog> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(this);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(CapturingLoggerProvider provider) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                var values = state is IEnumerable<KeyValuePair<string, object?>> properties
                    ? properties.ToDictionary(property => property.Key, property => property.Value)
                    : [];
                provider.Entries.Add(new CapturedLog(logLevel, formatter(state, exception), values));
            }
        }
    }

    private sealed record CapturedLog(
        LogLevel Level,
        string Message,
        IReadOnlyDictionary<string, object?> Values);

    private sealed class ThrowingAuditLogWriter : IAuditLogWriter
    {
        public Task WriteAsync(
            string eventType,
            string severity,
            string outcome,
            CancellationToken cancellationToken,
            Guid? actorUserId = null,
            string? actorUsername = null,
            string? targetType = null,
            string? targetId = null,
            object? details = null) =>
            throw new InvalidOperationException("database unavailable", new TestDbException());
    }

    private sealed class TestDbException() : DbException("database unavailable");
}
