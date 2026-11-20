using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;

namespace Somtumzing.Backend.Api.Pages.Admin;

public sealed class IndexModel(
    SignInManager<ApplicationUser> signInManager,
    IAuditLogWriter auditLogWriter,
    GameDbContext database) : PageModel
{
    public DashboardStats Stats { get; private set; } = new();
    public IReadOnlyList<RecentActivityRow> RecentActivities { get; private set; } = [];
    public IReadOnlyList<TopPlayerRow> TopPlayers { get; private set; } = [];

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

        Stats.TotalPlayers = await playerQuery.CountAsync(cancellationToken);
        Stats.ActivePlayers = await playerQuery.CountAsync(user => !user.IsSuspended, cancellationToken);
        Stats.SuspendedPlayers = await playerQuery.CountAsync(user => user.IsSuspended, cancellationToken);

        var sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-6);
        Stats.NewPlayersThisWeek = await playerQuery
            .CountAsync(user => user.CreatedAtUtc >= sevenDaysAgo, cancellationToken);

        Stats.TotalStages = await database.Stages.AsNoTracking().CountAsync(cancellationToken);
        Stats.TotalEvents = await database.GameEvents.AsNoTracking().CountAsync(cancellationToken);
        Stats.TotalMissions = await database.Missions.AsNoTracking().CountAsync(cancellationToken);
        Stats.TotalAchievements = await database.Achievements.AsNoTracking().CountAsync(cancellationToken);


        Stats.TotalGameSessions = await database.GameSessions.AsNoTracking().CountAsync(cancellationToken);
        Stats.SessionsToday = await database.GameSessions.AsNoTracking()
            .CountAsync(session => session.StartedDate >= DateTimeOffset.UtcNow.Date, cancellationToken);

        TopPlayers = await playerQuery
            .OrderByDescending(user => user.BestScore)
            .Take(5)
            .Select(user => new TopPlayerRow(
                user.Id,
                user.UserName ?? string.Empty,
                user.DisplayName,
                user.BestScore,
                user.Coins,
                user.TotalServed,
                user.IsSuspended))
            .ToListAsync(cancellationToken);

        RecentActivities = await database.AuditLogEntries.AsNoTracking()
            .OrderByDescending(entry => entry.OccurredAtUtc)
            .Take(8)
            .Select(entry => new RecentActivityRow(
                entry.OccurredAtUtc,
                entry.EventType,
                entry.Severity,
                entry.Outcome,
                entry.ActorUsername,
                entry.TargetType ?? string.Empty))
            .ToListAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostSignOutAsync()
    {
        if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
        {
            await auditLogWriter.WriteAsync(
                "AdminLogout",
                "Information",
                "Succeeded",
                HttpContext.RequestAborted,
                actorUserId: adminId,
                actorUsername: User.Identity?.Name,
                targetType: "BackOffice",
                targetId: adminId.ToString("D"));
        }

        await signInManager.SignOutAsync();
        return RedirectToPage("/Admin/Login");
    }

    public sealed record DashboardStats
    {
        public int TotalPlayers { get; set; }
        public int ActivePlayers { get; set; }
        public int SuspendedPlayers { get; set; }
        public int NewPlayersThisWeek { get; set; }
        public int TotalStages { get; set; }
        public int TotalEvents { get; set; }
        public int TotalMissions { get; set; }
        public int TotalAchievements { get; set; }
        public int TotalGameSessions { get; set; }
        public int SessionsToday { get; set; }
    }

    public sealed record TopPlayerRow(
        Guid Id,
        string Username,
        string? DisplayName,
        int BestScore,
        int Coins,
        int TotalServed,
        bool IsSuspended);

    public sealed record RecentActivityRow(
        DateTimeOffset OccurredAtUtc,
        string EventType,
        string Severity,
        string Outcome,
        string? ActorUsername,
        string TargetType);
}
