using Microsoft.AspNetCore.Identity;

namespace Somtumzing.Backend.Api.Models;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public bool IsSuspended { get; set; }
    public DateTimeOffset? SuspendedAtUtc { get; set; }
    public Guid? SuspendedByAdminId { get; set; }
    public string? SuspensionReason { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
}
