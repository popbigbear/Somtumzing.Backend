using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Pages.Admin;

[Authorize(Policy = "BackOffice")]
public class ReportsModel(GameDbContext database) : PageModel
{
    public PlayerGrowthReport PlayerGrowth { get; private set; } = new();
    public SessionReport Sessions { get; private set; } = new();
    public ContentReport Content { get; private set; } = new();
    public AuditSummaryReport AuditSummary { get; private set; } = new();
    public IReadOnlyList<StageProgressRow> StageProgress { get; private set; } = [];
    public IReadOnlyList<SessionDailyRow> SessionsByDay { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var playerRoleId = await database.Roles
            .Where(role => role.Name == "Player")
            .Select(role => (Guid?)role.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (playerRoleId is null) return;

        var playerQuery = database.Users
            .AsNoTracking()
            .Where(user => database.UserRoles.Any(userRole =>
                userRole.UserId == user.Id && userRole.RoleId == playerRoleId.Value));

        // Player growth - last 7 days
        var sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-6);
        var playerDates = await playerQuery
            .Where(user => user.CreatedAtUtc >= sevenDaysAgo)
            .Select(user => user.CreatedAtUtc.Date)
            .ToListAsync(cancellationToken);

        PlayerGrowth.DailyCounts = Enumerable.Range(0, 7)
            .Select(offset => {
                var date = DateTimeOffset.UtcNow.Date.AddDays(-6 + offset);
                return new DailyCountRow(date, playerDates.Count(d => d == date));
            })
            .ToList();

        PlayerGrowth.TotalPlayers = await playerQuery.CountAsync(cancellationToken);
        PlayerGrowth.SuspendedPlayers = await playerQuery.CountAsync(user => user.IsSuspended, cancellationToken);

        // Sessions
        Sessions.TotalSessions = await database.GameSessions.AsNoTracking().CountAsync(cancellationToken);
        Sessions.TodaySessions = await database.GameSessions.AsNoTracking()
            .CountAsync(session => session.StartedDate >= DateTimeOffset.UtcNow.Date, cancellationToken);
        Sessions.CompletedSessions = await database.GameSessions.AsNoTracking()
            .CountAsync(session => session.EndedDate != null, cancellationToken);
        Sessions.AvgNetProfit = await database.GameSessions.AsNoTracking()
            .Where(session => session.NetProfit != null)
            .Select(session => (decimal?)session.NetProfit!.Value)
            .AverageAsync(cancellationToken) ?? 0;

        // Sessions by day (last 7 days)
        var sessionDates = await database.GameSessions.AsNoTracking()
            .Where(session => session.StartedDate >= sevenDaysAgo)
            .Select(session => session.StartedDate.Date)
            .ToListAsync(cancellationToken);

        SessionsByDay = Enumerable.Range(0, 7)
            .Select(offset => {
                var date = DateTimeOffset.UtcNow.Date.AddDays(-6 + offset);
                return new SessionDailyRow(date, sessionDates.Count(d => d == date));
            })
            .ToList();

        // Content
        Content.TotalStages = await database.Stages.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalMissions = await database.Missions.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalAchievements = await database.Achievements.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalEvents = await database.GameEvents.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalUpgrades = await database.Upgrades.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalSabotageCards = await database.SabotageCards.AsNoTracking().CountAsync(cancellationToken);
        Content.TotalChaosEvents = await database.ChaosEventDefinitions.AsNoTracking().CountAsync(cancellationToken);
        Content.ActiveEvents = await database.GameEvents.AsNoTracking()
            .CountAsync(e => e.StartsAtUtc <= DateTimeOffset.UtcNow && e.EndsAtUtc >= DateTimeOffset.UtcNow, cancellationToken);

        // Stage progress (best stars per stage)
        StageProgress = await database.PlayerStageProgress.AsNoTracking()
            .GroupBy(progress => progress.StagePK)
            .Select(group => new StageProgressRow(
                group.Key,
                group.Count(),
                group.Max(progress => progress.BestStars),
                group.Max(progress => progress.BestNetProfit),
                group.Count(progress => progress.FirstCompletedDate != null)))
            .Take(20)
            .ToListAsync(cancellationToken);

        // Audit summary
        var last7Days = DateTimeOffset.UtcNow.AddDays(-7);
        AuditSummary.TotalEvents = await database.AuditLogEntries.AsNoTracking()
            .CountAsync(entry => entry.OccurredAtUtc >= last7Days, cancellationToken);
        AuditSummary.Warnings = await database.AuditLogEntries.AsNoTracking()
            .CountAsync(entry => entry.OccurredAtUtc >= last7Days && entry.Severity == "Warning", cancellationToken);
        AuditSummary.Errors = await database.AuditLogEntries.AsNoTracking()
            .CountAsync(entry => entry.OccurredAtUtc >= last7Days && (entry.Severity == "Error" || entry.Severity == "Critical"), cancellationToken);
        AuditSummary.InfoEvents = await database.AuditLogEntries.AsNoTracking()
            .CountAsync(entry => entry.OccurredAtUtc >= last7Days && entry.Severity == "Information", cancellationToken);
    }

    public sealed record PlayerGrowthReport
    {
        public int TotalPlayers { get; set; }
        public int SuspendedPlayers { get; set; }
        public IReadOnlyList<DailyCountRow> DailyCounts { get; set; } = [];
    }

    public sealed record SessionReport
    {
        public int TotalSessions { get; set; }
        public int TodaySessions { get; set; }
        public int CompletedSessions { get; set; }
        public decimal AvgNetProfit { get; set; }
    }

    public sealed record ContentReport
    {
        public int TotalStages { get; set; }
        public int TotalMissions { get; set; }
        public int TotalAchievements { get; set; }
        public int TotalEvents { get; set; }
        public int ActiveEvents { get; set; }
        public int TotalUpgrades { get; set; }
        public int TotalSabotageCards { get; set; }
        public int TotalChaosEvents { get; set; }
    }

    public sealed record AuditSummaryReport
    {
        public int TotalEvents { get; set; }
        public int InfoEvents { get; set; }
        public int Warnings { get; set; }
        public int Errors { get; set; }
    }

    public sealed record DailyCountRow(DateTimeOffset Date, int Count);
    public sealed record SessionDailyRow(DateTimeOffset Date, int Count);
    public sealed record StageProgressRow(Guid StagePK, int PlayerCount, int MaxStars, decimal MaxNetProfit, int CompletedCount);
}
