using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Services;

public sealed class AuditLogRetentionService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    IConfiguration configuration,
    ILogger<AuditLogRetentionService> logger) : BackgroundService
{
    private static readonly EventId RetentionCompletedEvent = new(2101, "AuditLogRetentionCompleted");
    private static readonly EventId RetentionFailedEvent = new(2102, "AuditLogRetentionFailed");
    private readonly int _retentionDays = configuration.GetValue<int?>("AuditLogs:RetentionDays") ?? 180;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunRetentionAsync(stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunRetentionAsync(stoppingToken);
        }
    }

    private async Task RunRetentionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            var cutoff = timeProvider.GetUtcNow().AddDays(-_retentionDays);
            var deleted = await database.AuditLogEntries
                .Where(entry => entry.OccurredAtUtc < cutoff)
                .ExecuteDeleteAsync(cancellationToken);

            logger.LogInformation(
                RetentionCompletedEvent,
                "Audit log retention completed. {EventType} {DeletedCount} {RetentionDays}",
                "AuditLogRetention",
                deleted,
                _retentionDays);
        }
        catch (DbException exception)
        {
            logger.LogError(
                RetentionFailedEvent,
                exception,
                "Audit log retention failed because the database is unavailable. {EventType}",
                "AuditLogRetention");
        }
        catch (InvalidOperationException exception) when (
            DatabaseExceptionClassifier.ContainsDbException(exception))
        {
            logger.LogError(
                RetentionFailedEvent,
                exception,
                "Audit log retention failed because the database is unavailable. {EventType}",
                "AuditLogRetention");
        }
    }
}
