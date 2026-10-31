using System.Data.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Somtumzing.Backend.Api.Services;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IServiceScopeFactory scopeFactory) : IExceptionHandler
{
    private static readonly EventId UnhandledExceptionEvent =
        new(5001, "UnhandledException");

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            UnhandledExceptionEvent,
            exception,
            "Request failed unexpectedly. {EventType} {RequestId}",
            "UnhandledException",
            httpContext.TraceIdentifier);

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var auditLogWriter = scope.ServiceProvider.GetRequiredService<IAuditLogWriter>();
            await auditLogWriter.WriteAsync(
                "UnhandledException",
                "Error",
                "Failed",
                cancellationToken,
                targetType: "Request",
                targetId: httpContext.TraceIdentifier,
                details: new { ExceptionType = exception.GetType().Name });
        }
        catch (DbException auditException)
        {
            logger.LogError(
                auditException,
                "Could not persist the unhandled exception audit event. {EventType} {RequestId}",
                "AuditPersistenceFailure",
                httpContext.TraceIdentifier);
        }
        catch (DbUpdateException auditException) when (
            DatabaseExceptionClassifier.ContainsDbException(auditException))
        {
            logger.LogError(
                auditException,
                "Could not persist the unhandled exception audit event. {EventType} {RequestId}",
                "AuditPersistenceFailure",
                httpContext.TraceIdentifier);
        }
        catch (InvalidOperationException auditException) when (
            DatabaseExceptionClassifier.ContainsDbException(auditException))
        {
            logger.LogError(
                auditException,
                "Could not persist the unhandled exception audit event. {EventType} {RequestId}",
                "AuditPersistenceFailure",
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        };

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return true;
    }
}
