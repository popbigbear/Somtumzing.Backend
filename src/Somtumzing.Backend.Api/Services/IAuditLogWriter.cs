namespace Somtumzing.Backend.Api.Services;

public interface IAuditLogWriter
{
    Task WriteAsync(
        string eventType,
        string severity,
        string outcome,
        CancellationToken cancellationToken,
        Guid? actorUserId = null,
        string? actorUsername = null,
        string? targetType = null,
        string? targetId = null,
        object? details = null);
}
