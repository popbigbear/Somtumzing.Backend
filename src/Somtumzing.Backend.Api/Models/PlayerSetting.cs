using System;

namespace Somtumzing.Backend.Api.Models;

/// <summary>
/// โมเดลเก็บข้อมูลการตั้งค่าของผู้เล่น (Player Settings)
/// </summary>
public sealed class PlayerSetting : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // ความสัมพันธ์กับผู้เล่น (UserPK)
    public Guid PlayerPK { get; set; }
    public bool Sound { get; set; } = true;
    public string Language { get; set; } = "th";
    public bool Analytics { get; set; } = true;

    // Navigation Property
    public ApplicationUser? User { get; set; }
}