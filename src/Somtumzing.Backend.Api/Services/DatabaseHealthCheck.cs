using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Services;

public sealed class DatabaseHealthCheck(
    GameDbContext database,
    ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    private static readonly EventId DatabaseFailureEvent = new(2001, "DatabaseFailure");

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var canConnect = await database.Database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            logger.LogWarning(
                DatabaseFailureEvent,
                "Database readiness check failed. {EventType} {Database}",
                "DatabaseFailure",
                "PostgreSQL");
        }

        return canConnect
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("PostgreSQL is unavailable.");
    }
}
