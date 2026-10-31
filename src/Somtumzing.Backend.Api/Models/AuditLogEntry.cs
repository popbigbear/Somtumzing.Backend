namespace Somtumzing.Backend.Api.Models;

public sealed class AuditLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = "RECORDED";
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public Guid? ActorUserId { get; set; }
    public string? ActorUsername { get; set; }
    public string? TargetType { get; set; }
    public string? TargetId { get; set; }
    public string? IpAddress { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string DetailsJson { get; set; } = "{}";
}
