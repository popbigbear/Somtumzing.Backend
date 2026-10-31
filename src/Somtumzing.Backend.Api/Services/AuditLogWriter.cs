using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Services;

public sealed class AuditLogWriter(
    GameDbContext database,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider timeProvider) : IAuditLogWriter
{
    private const int UsernameMaxLength = 256;
    private const int TargetTypeMaxLength = 64;
    private const int TargetIdMaxLength = 128;
    private const int RequestIdMaxLength = 128;

    public async Task WriteAsync(
        string eventType,
        string severity,
        string outcome,
        CancellationToken cancellationToken,
        Guid? actorUserId = null,
        string? actorUsername = null,
        string? targetType = null,
        string? targetId = null,
        object? details = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(severity);
        ArgumentException.ThrowIfNullOrWhiteSpace(outcome);

        var httpContext = httpContextAccessor.HttpContext;
        var principal = httpContext?.User;
        if (actorUserId is null)
        {
            var subject = principal?.FindFirstValue("sub")
                ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(subject, out var parsedUserId))
            {
                actorUserId = parsedUserId;
            }
        }

        var remoteIp = httpContext?.Connection.RemoteIpAddress;
        if (remoteIp?.IsIPv4MappedToIPv6 == true)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        database.AuditLogEntries.Add(new AuditLogEntry
        {
            OccurredAtUtc = timeProvider.GetUtcNow(),
            EventType = eventType,
            Severity = severity,
            Outcome = outcome,
            ActorUserId = actorUserId,
            ActorUsername = Truncate(actorUsername
                ?? principal?.FindFirstValue("name")
                ?? principal?.Identity?.Name, UsernameMaxLength),
            TargetType = Truncate(targetType, TargetTypeMaxLength),
            TargetId = Truncate(targetId, TargetIdMaxLength),
            IpAddress = remoteIp?.ToString(),
            RequestId = Truncate(httpContext?.TraceIdentifier, RequestIdMaxLength) ?? string.Empty,
            DetailsJson = details is null
                ? "{}"
                : JsonSerializer.Serialize(details)
        });

        await database.SaveChangesAsync(cancellationToken);
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is { Length: > 0 }
            ? value[..Math.Min(value.Length, maxLength)]
            : null;
}
