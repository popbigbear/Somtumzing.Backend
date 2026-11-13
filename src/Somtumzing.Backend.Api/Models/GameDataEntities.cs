namespace Somtumzing.Backend.Api.Models;

public interface IGameDataEntity
{
    Guid PK { get; set; }
    string Code { get; set; }
    DateTimeOffset CreatedDate { get; set; }
    Guid? CreatedBy { get; set; }
    DateTimeOffset UpdatedDate { get; set; }
    Guid? UpdatedBy { get; set; }
    string Status { get; set; }
}


// เพิ่ม Model สำหรับ Achievements
public sealed class GameAchievement : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AchievementKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TargetValue { get; set; }
    public int RewardCoins { get; set; }
    public int RewardDiamonds { get; set; }
}

public sealed class PlayerGameAchievement : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid PlayerPK { get; set; }
    public string AchievementKey { get; set; } = string.Empty;
    public int Progress { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsClaimed { get; set; }
}

// เพิ่ม Model สำหรับระบบ Daily Rewards
public sealed class PlayerDailyState : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid PlayerPK { get; set; }
    public int CurrentStreak { get; set; }
    public DateTimeOffset? LastClaimedDate { get; set; }
    public bool TodayClaimed { get; set; }
}

public sealed class CodeCounter : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PeriodYYMM { get; set; } = string.Empty;
    public string TableCode { get; set; } = string.Empty;
    public long NextNumber { get; set; }
}

public sealed class IdempotencyRecord : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ActorUserPK { get; set; }
    public string Scope { get; set; } = string.Empty;
    public Guid IdempotencyKey { get; set; }
    public string RequestHash { get; set; } = string.Empty;
    public string? ResourceType { get; set; }
    public Guid? ResourcePK { get; set; }
    public short? ResponseStatus { get; set; }
    public string? ResponseBody { get; set; }
    public DateTimeOffset ExpiresDate { get; set; }
}

public sealed class Stage : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StageKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int RequiredUnlockStars { get; set; }
    public int DefaultConfigVersion { get; set; }
}

public sealed class StageConfig : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid StagePK { get; set; }
    public int ConfigVersion { get; set; }
    public string RulesetKey { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public decimal? TargetSalesAmount { get; set; }
    public int MaxTableCapacity { get; set; }
    public int DeliveryQueueLimit { get; set; }
    public decimal? RawMaterialCostRate { get; set; }
    public string ChaosConfig { get; set; } = "{}";
    public DateTimeOffset? PublishedDate { get; set; }
}

public sealed class PlayerStageProgress : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid UserPK { get; set; }
    public Guid StagePK { get; set; }
    public int BestStars { get; set; }
    public int TotalStars { get; set; }
    public decimal BestNetProfit { get; set; }
    public DateTimeOffset? FirstCompletedDate { get; set; }
    public DateTimeOffset? LastPlayedDate { get; set; }
}

public sealed class GameSession : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid UserPK { get; set; }
    public Guid StagePK { get; set; }
    public int ConfigVersion { get; set; }
    public string RulesetKey { get; set; } = string.Empty;
    public string SessionType { get; set; } = string.Empty;
    public DateTimeOffset StartedDate { get; set; }
    public DateTimeOffset ExpectedEndDate { get; set; }
    public DateTimeOffset? EndedDate { get; set; }
    public DateTimeOffset? SettledDate { get; set; }
    public byte[]? ServerSeed { get; set; }
    public long SessionVersion { get; set; }
    public decimal? GrossRevenue { get; set; }
    public decimal? NetProfit { get; set; }
    public int? EarnedStars { get; set; }
    public decimal? DailyRating { get; set; }
}

public sealed class GameOrder : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SessionPK { get; set; }
    public int OrderSequence { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string MenuCode { get; set; } = string.Empty;
    public string RequestedTaste { get; set; } = "{}";
    public DateTimeOffset SpawnedDate { get; set; }
    public DateTimeOffset? PatienceDeadlineDate { get; set; }
    public DateTimeOffset? ResolvedDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool IsDelivery { get; set; }
}

public sealed class OrderResult : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid OrderPK { get; set; }
    public string PreparedTaste { get; set; } = "{}";
    public decimal BasePrice { get; set; }
    public decimal AccuracyBonus { get; set; }
    public decimal DeviationPenalty { get; set; }
    public decimal PaymentMultiplier { get; set; }
    public decimal DeliveryMultiplier { get; set; }
    public decimal FinalRevenue { get; set; }
    public decimal SatisfactionScore { get; set; }
    public int EarnedStars { get; set; }
    public string CalculationVersion { get; set; } = string.Empty;
}

public sealed class Wallet : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid UserPK { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

public sealed class WalletTransaction : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid WalletPK { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferencePK { get; set; }
    public string? ReferenceCode { get; set; }
    public string? Description { get; set; }
    public Guid? ReversalOfPK { get; set; }
}

public sealed class Upgrade : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UpgradeKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxLevel { get; set; }
    public string EffectConfig { get; set; } = "{}";
    
    // เพิ่ม Property นี้เพื่อให้ตรงกับคอลัมน์ description ในตาราง Upgrades
    public string? Description { get; set; } 
}

// เพิ่มคลาส PlayerUpgrade กลับเข้าไปให้ตรงกับ DDL จริงของตาราง PlayerUpgrades
public sealed class PlayerUpgrade : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    
    public Guid PlayerPK { get; set; }
    public Guid UpgradePK { get; set; }
    public int Level { get; set; }
    public DateTimeOffset? PurchasedDate { get; set; }

    // Navigation Property สำหรับ JOIN ตาราง Upgrades
    public Upgrade? Upgrade { get; set; }
}

public sealed class ChaosEventDefinition : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EventKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TriggerConfig { get; set; } = "{}";
    public decimal? ResponseWindowSeconds { get; set; }
    public string AllowedActions { get; set; } = "[]";
    public string FailureEffects { get; set; } = "{}";
    public string ActiveEffects { get; set; } = "{}";
    public string RulesetKey { get; set; } = string.Empty;
}

public sealed class ChaosEventInstance : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid SessionPK { get; set; }
    public Guid DefinitionPK { get; set; }
    public int EventSequence { get; set; }
    public DateTimeOffset TriggeredDate { get; set; }
    public DateTimeOffset? RespondByDate { get; set; }
    public DateTimeOffset? ResolvedDate { get; set; }
    public Guid? ResolvedByUserPK { get; set; }
    public string? ActionCode { get; set; }
    public string? ResultCode { get; set; }
    public string EffectSnapshot { get; set; } = "{}";
    public string SourceType { get; set; } = string.Empty;
    public string? SourceCode { get; set; }
}

public sealed class Challenge : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ChallengeKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid StagePK { get; set; }
    public int ConfigVersion { get; set; }
    public string RulesetKey { get; set; } = string.Empty;
    public DateTimeOffset StartsDate { get; set; }
    public DateTimeOffset EndsDate { get; set; }
    public int? AttemptLimit { get; set; }
    public string RewardConfig { get; set; } = "{}";
}

public sealed class ChallengeRun : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ChallengePK { get; set; }
    public Guid UserPK { get; set; }
    public int RunSequence { get; set; }
    public DateTimeOffset StartedDate { get; set; }
    public DateTimeOffset ExpiresDate { get; set; }
    public DateTimeOffset? SubmittedDate { get; set; }
    public DateTimeOffset? ValidatedDate { get; set; }
    public string RunTicketHash { get; set; } = string.Empty;
    public string? EventEvidence { get; set; }
    public decimal? CalculatedScore { get; set; }
    public decimal? CalculatedNetProfit { get; set; }
    public int? CalculatedStars { get; set; }
    public string? ValidationCode { get; set; }
    public int? RankSnapshot { get; set; }
}

public sealed class GameMatch : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string RulesetKey { get; set; } = string.Empty;
    public string SeedHash { get; set; } = string.Empty;
    public DateTimeOffset? StartedDate { get; set; }
    public DateTimeOffset? ExpectedEndDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }
    public short? WinnerTeam { get; set; }
    public string? ResultCode { get; set; }
    public long Version { get; set; }
}

public sealed class MatchParticipant : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid MatchPK { get; set; }
    public Guid UserPK { get; set; }
    public short TeamNumber { get; set; }
    public int? RatingBefore { get; set; }
    public int? RatingAfter { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public DateTimeOffset? DisconnectedDate { get; set; }
    public string? DeckSnapshot { get; set; }
    public string? DefenseCardCode { get; set; }
}

public sealed class MatchAction : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid MatchPK { get; set; }
    public Guid ParticipantPK { get; set; }
    public long SequenceNumber { get; set; }
    public string ActionCode { get; set; } = string.Empty;
    public string? TargetCode { get; set; }
    public string ActionData { get; set; } = "{}";
    public DateTimeOffset ServerReceivedDate { get; set; }
    public DateTimeOffset? ClientOccurredDate { get; set; }
    public bool Accepted { get; set; }
    public string? RejectCode { get; set; }
    public long MatchVersion { get; set; }
}

public sealed class MatchResult : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid MatchPK { get; set; }
    public Guid ParticipantPK { get; set; }
    public short TeamNumber { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal NetProfit { get; set; }
    public int EarnedStars { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public int? RatingChange { get; set; }
    public DateTimeOffset FinalizedDate { get; set; }
}

public sealed class SabotageCard : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CardKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public short? Tier { get; set; }
    public decimal EnergyCostPct { get; set; }
    public decimal EffectDurationSeconds { get; set; }
    public string? CounterActionCode { get; set; }
    public string EffectConfig { get; set; } = "{}";
    public string RulesetKey { get; set; } = string.Empty;
}

public sealed class PlayerLoadout : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid UserPK { get; set; }
    public string RulesetKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public sealed class PlayerLoadoutCard : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid LoadoutPK { get; set; }
    public Guid CardPK { get; set; }
    public string SlotType { get; set; } = string.Empty;
    public short SlotNumber { get; set; }
}

public sealed class PlayerMission : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid PlayerPK { get; set; }
    public string MissionKey { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int TargetValue { get; set; }
    
    // เพิ่ม Properties เหล่านี้เพื่อให้ตรงกับการใช้งานใน Controller
    public bool IsCompleted { get; set; }
    public bool IsClaimed { get; set; }
}

// เพิ่ม Master Model สำหรับตาราง Missions
public sealed class Mission : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MissionKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RewardCoins { get; set; }
}

public sealed class MatchSabotageEvent : IGameDataEntity
{
    public Guid PK { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid MatchPK { get; set; }
    public Guid SourceParticipantPK { get; set; }
    public Guid TargetParticipantPK { get; set; }
    public Guid CardPK { get; set; }
    public long EventSequence { get; set; }
    public DateTimeOffset TriggeredDate { get; set; }
    public DateTimeOffset? RespondByDate { get; set; }
    public DateTimeOffset? ResolvedDate { get; set; }
    public string? CounterActionCode { get; set; }
    public string? ResultCode { get; set; }
    public decimal EnergySpentPct { get; set; }
    public string EffectSnapshot { get; set; } = "{}";
    public DateTimeOffset? ImmunityUntilDate { get; set; }
}


