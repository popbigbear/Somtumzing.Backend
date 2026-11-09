using System;

namespace Somtumzing.Backend.Api.Models;

public sealed class PlayerCosmetic : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Guid PlayerPK { get; set; }
    public string CosmeticKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool Unlocked { get; set; } = false;
}