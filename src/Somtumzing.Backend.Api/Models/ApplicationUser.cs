using Microsoft.AspNetCore.Identity;

namespace Somtumzing.Backend.Api.Models;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? Code { get; set; }
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
    
    // เพิ่มฟิลด์ DisplayName สำหรับแสดงผลในเกมโดยเฉพาะ (แยกจาก UserName)
    public string? DisplayName { get; set; }

    // เพิ่มฟิลด์ใหม่สำหรับเก็บข้อมูลจริงของผู้เล่น
    public int Coins { get; set; } = 100;
    public int Energy { get; set; } = 10;
    public int BestScore { get; set; } = 0;
    public int TotalServed { get; set; } = 0;

    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
}
