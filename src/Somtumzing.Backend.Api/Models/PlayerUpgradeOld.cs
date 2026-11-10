using System;

namespace Somtumzing.Backend.Api.Models;

/// <summary>
/// โมเดลเก็บข้อมูลเลเวลอัปเกรดของผู้เล่น (PlayerUpgrades)
/// </summary>
public sealed class PlayerUpgradeOld : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Guid PlayerPK { get; set; } // ตรงกับ DDL "PlayerPK"
    public Guid UpgradePK { get; set; } // ตรงกับ DDL "UpgradePK"
    public int Level { get; set; } = 0;
    public DateTimeOffset? PurchasedDate { get; set; }

    public Upgrade? Upgrade { get; set; }
}