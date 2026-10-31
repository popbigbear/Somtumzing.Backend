using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Data;

public sealed class GameDbContext(DbContextOptions<GameDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>,
        ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
    public DbSet<CodeCounter> CodeCounters => Set<CodeCounter>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<StageConfig> StageConfigs => Set<StageConfig>();
    public DbSet<PlayerStageProgress> PlayerStageProgress => Set<PlayerStageProgress>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<GameOrder> Orders => Set<GameOrder>();
    public DbSet<OrderResult> OrderResults => Set<OrderResult>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Upgrade> Upgrades => Set<Upgrade>();
    public DbSet<PlayerUpgrade> PlayerUpgrades => Set<PlayerUpgrade>();
    public DbSet<ChaosEventDefinition> ChaosEventDefinitions => Set<ChaosEventDefinition>();
    public DbSet<ChaosEventInstance> ChaosEventInstances => Set<ChaosEventInstance>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeRun> ChallengeRuns => Set<ChallengeRun>();
    public DbSet<GameMatch> Matches => Set<GameMatch>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
    public DbSet<MatchAction> MatchActions => Set<MatchAction>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();
    public DbSet<SabotageCard> SabotageCards => Set<SabotageCard>();
    public DbSet<PlayerLoadout> PlayerLoadouts => Set<PlayerLoadout>();
    public DbSet<PlayerLoadoutCard> PlayerLoadoutCards => Set<PlayerLoadoutCard>();
    public DbSet<MatchSabotageEvent> MatchSabotageEvents => Set<MatchSabotageEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureUser(builder);
        ConfigureRole(builder);
        ConfigureUserRole(builder);
        ConfigureRefreshToken(builder);
        ConfigureAuditLog(builder);
        ConfigureGameTables(builder);
    }

    private static void ConfigureUser(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", table =>
                table.HasCheckConstraint("CK_Users_Status",
                    "\"Status\" IN ('ACTIVE', 'SUSPENDED', 'DISABLED', 'DELETED')"));
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(user => user.Code).HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
            entity.HasIndex(user => user.Code).IsUnique();
            entity.Property(user => user.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(user => user.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(user => user.Status).HasMaxLength(24).HasDefaultValue("ACTIVE").IsRequired();
            entity.Property(user => user.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(user => user.SuspendedAtUtc).HasColumnName("SuspendedDate");
            entity.Property(user => user.SuspendedByAdminId).HasColumnName("SuspendedByPK");
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(user => user.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(user => user.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(user => user.SuspendedByAdminId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRole(ModelBuilder builder)
    {
        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("AspNetRoles", table =>
                table.HasCheckConstraint("CK_AspNetRoles_Status", "\"Status\" IN ('ACTIVE', 'INACTIVE')"));
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(role => role.Code).HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
            entity.HasIndex(role => role.Code).IsUnique();
            entity.Property(role => role.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(role => role.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(role => role.Status).HasMaxLength(24).HasDefaultValue("ACTIVE").IsRequired();
            entity.Property(role => role.Description).HasMaxLength(256);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(role => role.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(role => role.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUserRole(ModelBuilder builder)
    {
        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("AspNetUserRoles", table =>
                table.HasCheckConstraint("CK_AspNetUserRoles_Status", "\"Status\" IN ('ACTIVE', 'REVOKED')"));
            entity.HasKey(role => role.PK);
            entity.Property(role => role.PK).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(role => role.UserId).HasColumnName("UserPK");
            entity.Property(role => role.RoleId).HasColumnName("RolePK");
            entity.Property(role => role.Code).HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
            entity.HasIndex(role => role.Code).IsUnique();
            entity.HasIndex(role => new { role.UserId, role.RoleId }).IsUnique();
            entity.Property(role => role.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(role => role.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(role => role.Status).HasMaxLength(24).HasDefaultValue("ACTIVE").IsRequired();
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(role => role.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(role => role.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRefreshToken(ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens", table =>
                table.HasCheckConstraint("CK_RefreshTokens_Status",
                    "\"Status\" IN ('ACTIVE', 'REVOKED', 'EXPIRED')"));
            entity.HasKey(token => token.Id);
            entity.Property(token => token.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(token => token.Code).HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
            entity.HasIndex(token => token.Code).IsUnique();
            entity.Property(token => token.UserId).HasColumnName("UserPK");
            entity.Property(token => token.CreatedAtUtc).HasColumnName("CreatedTokenDate");
            entity.Property(token => token.ExpiresAtUtc).HasColumnName("ExpiresDate");
            entity.Property(token => token.RevokedAtUtc).HasColumnName("RevokedDate").IsConcurrencyToken();
            entity.Property(token => token.ReplacedByTokenId).HasColumnName("ReplacedByPK");
            entity.Property(token => token.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(token => token.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(token => token.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(token => token.Status).HasMaxLength(24).HasDefaultValue("ACTIVE").IsRequired();
            entity.HasIndex(token => token.TokenHash).IsUnique();
            entity.HasIndex(token => token.UserId);
            entity.HasOne(token => token.User).WithMany(user => user.RefreshTokens)
                .HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<RefreshToken>().WithMany().HasForeignKey(token => token.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(token => token.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(token => token.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAuditLog(ModelBuilder builder)
    {
        builder.Entity<AuditLogEntry>(entity =>
        {
            entity.ToTable("AuditLogEntries", table =>
                table.HasCheckConstraint("CK_AuditLogEntries_Status", "\"Status\" = 'RECORDED'"));
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(entry => entry.Code).HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
            entity.HasIndex(entry => entry.Code).IsUnique();
            entity.Property(entry => entry.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(entry => entry.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(entry => entry.Status).HasMaxLength(24).HasDefaultValue("RECORDED").IsRequired();
            entity.Property(entry => entry.EventType).HasMaxLength(64).IsRequired();
            entity.Property(entry => entry.Severity).HasMaxLength(16).IsRequired();
            entity.Property(entry => entry.Outcome).HasMaxLength(64).IsRequired();
            entity.Property(entry => entry.ActorUsername).HasMaxLength(256);
            entity.Property(entry => entry.TargetType).HasMaxLength(64);
            entity.Property(entry => entry.TargetId).HasMaxLength(128);
            entity.Property(entry => entry.IpAddress).HasMaxLength(45);
            entity.Property(entry => entry.RequestId).HasMaxLength(128).IsRequired();
            entity.Property(entry => entry.DetailsJson).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(entry => entry.OccurredAtUtc);
            entity.HasIndex(entry => new { entry.EventType, entry.OccurredAtUtc });
            entity.HasIndex(entry => new { entry.Severity, entry.OccurredAtUtc });
            entity.HasIndex(entry => new { entry.ActorUsername, entry.OccurredAtUtc });
            entity.HasIndex(entry => new { entry.IpAddress, entry.OccurredAtUtc });
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(entry => entry.ActorUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(entry => entry.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(entry => entry.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureGameTables(ModelBuilder builder)
    {
        ConfigureRecord<CodeCounter>(builder, "CodeCounters", "Sequence", "0", "ACTIVE");
        builder.Entity<CodeCounter>(entity =>
        {
            entity.Property(row => row.PeriodYYMM).HasMaxLength(4).IsFixedLength().IsRequired();
            entity.Property(row => row.TableCode).HasMaxLength(1).IsFixedLength().IsRequired();
            entity.HasIndex(row => new { row.PeriodYYMM, row.TableCode }).IsUnique();
        });

        ConfigureRecord<IdempotencyRecord>(builder, "IdempotencyRecords", "Idempotency", "F",
            "IN_PROGRESS", "COMPLETED", "FAILED");
        builder.Entity<IdempotencyRecord>(entity =>
        {
            entity.Property(row => row.Scope).HasMaxLength(100).IsRequired();
            entity.Property(row => row.RequestHash).HasMaxLength(64).IsFixedLength().IsRequired();
            entity.Property(row => row.ResourceType).HasMaxLength(64);
            entity.Property(row => row.ResponseBody).HasColumnType("jsonb");
            entity.HasIndex(row => new { row.ActorUserPK, row.Scope, row.IdempotencyKey }).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.ActorUserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<Stage>(builder, "Stages", "Stage", "G", "DRAFT", "PUBLISHED", "RETIRED");
        builder.Entity<Stage>(entity =>
        {
            entity.Property(row => row.StageKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.DisplayName).HasMaxLength(160).IsRequired();
            entity.HasIndex(row => row.StageKey).IsUnique();
        });

        ConfigureRecord<StageConfig>(builder, "StageConfigs", "StageConfig", "H",
            "DRAFT", "PUBLISHED", "RETIRED");
        builder.Entity<StageConfig>(entity =>
        {
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.TargetSalesAmount).HasPrecision(12, 2);
            entity.Property(row => row.RawMaterialCostRate).HasPrecision(7, 6);
            entity.Property(row => row.ChaosConfig).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(row => new { row.StagePK, row.ConfigVersion }).IsUnique();
            entity.HasOne<Stage>().WithMany().HasForeignKey(row => row.StagePK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<PlayerStageProgress>(builder, "PlayerStageProgress", "Progress", "I",
            "LOCKED", "UNLOCKED", "COMPLETED");
        builder.Entity<PlayerStageProgress>(entity =>
        {
            entity.Property(row => row.BestNetProfit).HasPrecision(12, 2);
            entity.HasIndex(row => new { row.UserPK, row.StagePK }).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Stage>().WithMany().HasForeignKey(row => row.StagePK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable("PlayerStageProgress", table =>
                table.HasCheckConstraint("CK_PlayerStageProgress_NonNegative",
                    "\"BestStars\" >= 0 AND \"TotalStars\" >= 0"));
        });

        ConfigureRecord<GameSession>(builder, "GameSessions", "Session", "J",
            "ACTIVE", "COMPLETED", "SETTLED", "ABANDONED", "INVALID");
        builder.Entity<GameSession>(entity =>
        {
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.SessionType).HasMaxLength(24).IsRequired();
            entity.Property(row => row.GrossRevenue).HasPrecision(12, 2);
            entity.Property(row => row.NetProfit).HasPrecision(12, 2);
            entity.Property(row => row.DailyRating).HasPrecision(3, 2);
            entity.HasIndex(row => new { row.UserPK, row.StartedDate });
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<StageConfig>().WithMany()
                .HasForeignKey(row => new { row.StagePK, row.ConfigVersion })
                .HasPrincipalKey(row => new { row.StagePK, row.ConfigVersion })
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<GameOrder>(builder, "Orders", "Order", "K",
            "QUEUED", "IN_PROGRESS", "SERVED", "CANCELLED", "DROPPED", "EXPIRED");
        builder.Entity<GameOrder>(entity =>
        {
            entity.Property(row => row.OrderType).HasMaxLength(24).IsRequired();
            entity.Property(row => row.MenuCode).HasMaxLength(64).IsRequired();
            entity.Property(row => row.RequestedTaste).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.PaymentMethod).HasMaxLength(32).IsRequired();
            entity.HasIndex(row => new { row.SessionPK, row.OrderSequence }).IsUnique();
            entity.HasOne<GameSession>().WithMany().HasForeignKey(row => row.SessionPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<OrderResult>(builder, "OrderResults", "OrderResult", "L", "POSTED", "VOIDED");
        builder.Entity<OrderResult>(entity =>
        {
            entity.Property(row => row.PreparedTaste).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.BasePrice).HasPrecision(12, 2);
            entity.Property(row => row.AccuracyBonus).HasPrecision(12, 2);
            entity.Property(row => row.DeviationPenalty).HasPrecision(12, 2);
            entity.Property(row => row.PaymentMultiplier).HasPrecision(8, 4);
            entity.Property(row => row.DeliveryMultiplier).HasPrecision(8, 4);
            entity.Property(row => row.FinalRevenue).HasPrecision(12, 2);
            entity.Property(row => row.SatisfactionScore).HasPrecision(5, 2);
            entity.Property(row => row.CalculationVersion).HasMaxLength(64).IsRequired();
            entity.HasIndex(row => row.OrderPK).IsUnique();
            entity.HasOne<GameOrder>().WithMany().HasForeignKey(row => row.OrderPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<Wallet>(builder, "Wallets", "Wallet", "M", "ACTIVE", "FROZEN", "CLOSED");
        builder.Entity<Wallet>(entity =>
        {
            entity.Property(row => row.CurrencyCode).HasMaxLength(32).IsRequired();
            entity.Property(row => row.Balance).HasPrecision(20, 4);
            entity.HasIndex(row => new { row.UserPK, row.CurrencyCode }).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<WalletTransaction>(builder, "WalletTransactions", "WalletTxn", "N",
            "POSTED", "REVERSED");
        builder.Entity<WalletTransaction>(entity =>
        {
            entity.Property(row => row.TransactionType).HasMaxLength(32).IsRequired();
            entity.Property(row => row.Amount).HasPrecision(20, 4);
            entity.Property(row => row.BalanceAfter).HasPrecision(20, 4);
            entity.Property(row => row.ReferenceType).HasMaxLength(64);
            entity.Property(row => row.ReferenceCode).HasMaxLength(40);
            entity.Property(row => row.Description).HasMaxLength(256);
            entity.HasIndex(row => new { row.WalletPK, row.CreatedDate });
            entity.HasOne<Wallet>().WithMany().HasForeignKey(row => row.WalletPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<WalletTransaction>().WithMany().HasForeignKey(row => row.ReversalOfPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<Upgrade>(builder, "Upgrades", "Upgrade", "O", "DRAFT", "PUBLISHED", "RETIRED");
        builder.Entity<Upgrade>(entity =>
        {
            entity.Property(row => row.UpgradeKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(row => row.Category).HasMaxLength(32).IsRequired();
            entity.Property(row => row.CurrencyCode).HasMaxLength(32).IsRequired();
            entity.Property(row => row.Price).HasPrecision(20, 4);
            entity.Property(row => row.EffectConfig).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(row => row.UpgradeKey).IsUnique();
        });

        ConfigureRecord<PlayerUpgrade>(builder, "PlayerUpgrades", "PlayerUpgrade", "P", "ACTIVE", "INACTIVE");
        builder.Entity<PlayerUpgrade>(entity =>
        {
            entity.HasIndex(row => new { row.UserPK, row.UpgradePK }).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Upgrade>().WithMany().HasForeignKey(row => row.UpgradePK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<ChaosEventDefinition>(builder, "ChaosEventDefinitions", "ChaosDef", "Q",
            "DRAFT", "PUBLISHED", "RETIRED");
        builder.Entity<ChaosEventDefinition>(entity =>
        {
            entity.Property(row => row.EventKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(row => row.TriggerConfig).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.ResponseWindowSeconds).HasPrecision(7, 3);
            entity.Property(row => row.AllowedActions).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.FailureEffects).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.ActiveEffects).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.HasIndex(row => new { row.RulesetKey, row.EventKey }).IsUnique();
        });

        ConfigureRecord<ChaosEventInstance>(builder, "ChaosEventInstances", "ChaosEvent", "R",
            "WARNING", "ACTIVE", "RESOLVED", "FAILED", "EXPIRED", "CANCELLED");
        builder.Entity<ChaosEventInstance>(entity =>
        {
            entity.Property(row => row.ActionCode).HasMaxLength(64);
            entity.Property(row => row.ResultCode).HasMaxLength(32);
            entity.Property(row => row.EffectSnapshot).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.SourceType).HasMaxLength(32).IsRequired();
            entity.Property(row => row.SourceCode).HasMaxLength(40);
            entity.HasIndex(row => new { row.SessionPK, row.EventSequence }).IsUnique();
            entity.HasOne<GameSession>().WithMany().HasForeignKey(row => row.SessionPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ChaosEventDefinition>().WithMany().HasForeignKey(row => row.DefinitionPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.ResolvedByUserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<Challenge>(builder, "Challenges", "Challenge", "S",
            "DRAFT", "SCHEDULED", "ACTIVE", "CLOSED", "CANCELLED");
        builder.Entity<Challenge>(entity =>
        {
            entity.Property(row => row.ChallengeKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.RewardConfig).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(row => row.ChallengeKey).IsUnique();
            entity.HasOne<StageConfig>().WithMany()
                .HasForeignKey(row => new { row.StagePK, row.ConfigVersion })
                .HasPrincipalKey(row => new { row.StagePK, row.ConfigVersion })
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<ChallengeRun>(builder, "ChallengeRuns", "ChallengeRun", "T",
            "ACTIVE", "SUBMITTED", "VALIDATING", "ACCEPTED", "REJECTED", "EXPIRED");
        builder.Entity<ChallengeRun>(entity =>
        {
            entity.Property(row => row.RunTicketHash).HasMaxLength(64).IsFixedLength().IsRequired();
            entity.Property(row => row.EventEvidence).HasColumnType("jsonb");
            entity.Property(row => row.CalculatedScore).HasPrecision(12, 2);
            entity.Property(row => row.CalculatedNetProfit).HasPrecision(12, 2);
            entity.Property(row => row.ValidationCode).HasMaxLength(32);
            entity.HasIndex(row => new { row.ChallengePK, row.UserPK, row.RunSequence }).IsUnique();
            entity.HasIndex(row => new { row.ChallengePK, row.Status, row.CalculatedScore, row.SubmittedDate });
            entity.HasOne<Challenge>().WithMany().HasForeignKey(row => row.ChallengePK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<GameMatch>(builder, "Matches", "Match", "U",
            "FORMING", "READY", "IN_PROGRESS", "COMPLETED", "FORFEIT", "INVALID", "CANCELLED");
        builder.Entity<GameMatch>(entity =>
        {
            entity.Property(row => row.Mode).HasMaxLength(24).IsRequired();
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.SeedHash).HasMaxLength(64).IsFixedLength().IsRequired();
            entity.Property(row => row.ResultCode).HasMaxLength(24);
        });

        ConfigureRecord<MatchParticipant>(builder, "MatchParticipants", "Participant", "V",
            "ACTIVE", "DISCONNECTED", "FORFEIT", "COMPLETED");
        builder.Entity<MatchParticipant>(entity =>
        {
            entity.Property(row => row.DeckSnapshot).HasColumnType("jsonb");
            entity.Property(row => row.DefenseCardCode).HasMaxLength(64);
            entity.HasIndex(row => new { row.MatchPK, row.UserPK }).IsUnique();
            entity.HasIndex(row => new { row.UserPK, row.JoinedDate });
            entity.HasOne<GameMatch>().WithMany().HasForeignKey(row => row.MatchPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<MatchAction>(builder, "MatchActions", "MatchAction", "W", "ACCEPTED", "REJECTED");
        builder.Entity<MatchAction>(entity =>
        {
            entity.Property(row => row.ActionCode).HasMaxLength(64).IsRequired();
            entity.Property(row => row.TargetCode).HasMaxLength(40);
            entity.Property(row => row.ActionData).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.RejectCode).HasMaxLength(64);
            entity.HasIndex(row => new { row.MatchPK, row.ParticipantPK, row.SequenceNumber }).IsUnique();
            entity.HasOne<GameMatch>().WithMany().HasForeignKey(row => row.MatchPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<MatchParticipant>().WithMany().HasForeignKey(row => row.ParticipantPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<MatchResult>(builder, "MatchResults", "MatchResult", "X", "FINAL", "INVALIDATED");
        builder.Entity<MatchResult>(entity =>
        {
            entity.Property(row => row.GrossRevenue).HasPrecision(12, 2);
            entity.Property(row => row.NetProfit).HasPrecision(12, 2);
            entity.Property(row => row.Outcome).HasMaxLength(24).IsRequired();
            entity.HasIndex(row => new { row.MatchPK, row.ParticipantPK }).IsUnique();
            entity.HasOne<GameMatch>().WithMany().HasForeignKey(row => row.MatchPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<MatchParticipant>().WithMany().HasForeignKey(row => row.ParticipantPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<SabotageCard>(builder, "SabotageCards", "SabotageCard", "Y",
            "DRAFT", "PUBLISHED", "RETIRED");
        builder.Entity<SabotageCard>(entity =>
        {
            entity.Property(row => row.CardKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.DisplayName).HasMaxLength(160).IsRequired();
            entity.Property(row => row.Kind).HasMaxLength(16).IsRequired();
            entity.Property(row => row.EnergyCostPct).HasPrecision(5, 2);
            entity.Property(row => row.EffectDurationSeconds).HasPrecision(7, 3);
            entity.Property(row => row.CounterActionCode).HasMaxLength(64);
            entity.Property(row => row.EffectConfig).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.HasIndex(row => new { row.RulesetKey, row.CardKey }).IsUnique();
        });

        ConfigureRecord<PlayerLoadout>(builder, "PlayerLoadouts", "Loadout", "Z", "ACTIVE", "INACTIVE");
        builder.Entity<PlayerLoadout>(entity =>
        {
            entity.Property(row => row.RulesetKey).HasMaxLength(64).IsRequired();
            entity.Property(row => row.Name).HasMaxLength(64).IsRequired();
            entity.HasIndex(row => new { row.UserPK, row.RulesetKey, row.Name }).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(row => row.UserPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<PlayerLoadoutCard>(builder, "PlayerLoadoutCards", "LoadoutCard", "0",
            "ACTIVE", "REMOVED");
        builder.Entity<PlayerLoadoutCard>(entity =>
        {
            entity.Property(row => row.SlotType).HasMaxLength(16).IsRequired();
            entity.HasIndex(row => new { row.LoadoutPK, row.SlotType, row.SlotNumber }).IsUnique();
            entity.HasIndex(row => new { row.LoadoutPK, row.CardPK, row.SlotType }).IsUnique();
            entity.HasOne<PlayerLoadout>().WithMany().HasForeignKey(row => row.LoadoutPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<SabotageCard>().WithMany().HasForeignKey(row => row.CardPK)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureRecord<MatchSabotageEvent>(builder, "MatchSabotageEvents", "SabotageEvent", "1",
            "WARNING", "ACTIVE", "COUNTERED", "FAILED", "EXPIRED", "BLOCKED");
        builder.Entity<MatchSabotageEvent>(entity =>
        {
            entity.Property(row => row.CounterActionCode).HasMaxLength(64);
            entity.Property(row => row.ResultCode).HasMaxLength(32);
            entity.Property(row => row.EnergySpentPct).HasPrecision(5, 2);
            entity.Property(row => row.EffectSnapshot).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(row => new { row.MatchPK, row.EventSequence }).IsUnique();
            entity.HasOne<GameMatch>().WithMany().HasForeignKey(row => row.MatchPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<MatchParticipant>().WithMany().HasForeignKey(row => row.SourceParticipantPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<MatchParticipant>().WithMany().HasForeignKey(row => row.TargetParticipantPK)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<SabotageCard>().WithMany().HasForeignKey(row => row.CardPK)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRecord<TEntity>(
        ModelBuilder builder,
        string table,
        string codePrefix,
        string tableCode,
        params string[] statuses)
        where TEntity : class, IGameDataEntity
    {
        var entity = builder.Entity<TEntity>();
        entity.ToTable(table, tableBuilder =>
            tableBuilder.HasCheckConstraint(
                $"CK_{table}_Status",
                $"\"Status\" IN ({string.Join(", ", statuses.Select(status => $"'{status}'"))})"));
        entity.HasKey("PK");
        entity.Property<Guid>("PK").HasDefaultValueSql("gen_random_uuid()");
        entity.Property<string>("Code").HasMaxLength(40).IsRequired().ValueGeneratedOnAdd();
        entity.HasIndex("Code").IsUnique();
        entity.Property<DateTimeOffset>("CreatedDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property<DateTimeOffset>("UpdatedDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property<string>("Status").HasMaxLength(24).IsRequired();
        entity.HasOne<ApplicationUser>().WithMany().HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<ApplicationUser>().WithMany().HasForeignKey("UpdatedBy")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
