namespace Somtumzing.Backend.Api.Models;

public sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
