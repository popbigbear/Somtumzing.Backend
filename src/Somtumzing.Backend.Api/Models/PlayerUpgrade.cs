namespace Somtumzing.Backend.Api.Models;

/// <summary>
/// โมเดลเก็บข้อมูลการซื้ออัปเกรดของผู้เล่น (PlayerUpgrades)
/// </summary>
public sealed class PlayerUpgrade : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // คอลัมน์จริงในตารางเดิมของคุณ
    public Guid PlayerPK { get; set; }
    public Guid UpgradePK { get; set; }
    public int Level { get; set; } = 0;
    public DateTimeOffset? PurchasedDate { get; set; }
}