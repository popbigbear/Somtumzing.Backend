using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;

namespace Somtumzing.Backend.Api.Pages.Admin.Players;

public sealed class IndexModel(
    GameDbContext database,
    UserManager<ApplicationUser> userManager,
    IAuditLogWriter auditLogWriter) : PageModel
{
    private const int PageSize = 25;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "All";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty]
    public ModerationInput Input { get; set; } = new();

    public IReadOnlyList<PlayerRow> Players { get; private set; } = [];
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public int CurrentPage => Math.Clamp(PageNumber, 1, TotalPages);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadPlayersAsync(cancellationToken);
    }

    public Task<IActionResult> OnPostSuspendAsync(CancellationToken cancellationToken) =>
        SetSuspensionAsync(suspend: true, cancellationToken);

    public Task<IActionResult> OnPostReactivateAsync(CancellationToken cancellationToken) =>
        SetSuspensionAsync(suspend: false, cancellationToken);

    private async Task<IActionResult> SetSuspensionAsync(
        bool suspend,
        CancellationToken cancellationToken)
    {
        Input.Reason = Input.Reason?.Trim() ?? string.Empty;
        if (Input.UserId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(Input.UserId), "Select a player account.");
        }

        if (Input.Reason.Length < 3)
        {
            ModelState.AddModelError(nameof(Input.Reason), "Enter a reason with at least 3 characters.");
        }

        if (!ModelState.IsValid)
        {
            await LoadPlayersAsync(cancellationToken);
            return Page();
        }

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
        {
            return Forbid();
        }

        var playerRoleId = await database.Roles
            .Where(role => role.Name == "Player")
            .Select(role => (Guid?)role.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (playerRoleId is null)
        {
            throw new InvalidOperationException("The Player role has not been initialized.");
        }

        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var player = await database.Users.SingleOrDefaultAsync(
            user => user.Id == Input.UserId
                && database.UserRoles.Any(userRole =>
                    userRole.UserId == user.Id && userRole.RoleId == playerRoleId.Value),
            cancellationToken);
        if (player is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            TempData["StatusMessage"] = "That player account could not be found.";
            return RedirectToPage(new { search = Search, status = Status, pageNumber = PageNumber });
        }

        if (player.IsSuspended == suspend)
        {
            await transaction.RollbackAsync(cancellationToken);
            TempData["StatusMessage"] = suspend
                ? "This player is already suspended."
                : "This player is already active.";
            return RedirectToPage(new { search = Search, status = Status, pageNumber = PageNumber });
        }

        var now = DateTimeOffset.UtcNow;
        player.IsSuspended = suspend;
        player.Status = suspend ? "SUSPENDED" : "ACTIVE";
        player.SuspendedAtUtc = suspend ? now : null;
        player.SuspendedByAdminId = suspend ? adminId : null;
        player.SuspensionReason = suspend ? Input.Reason : null;
        player.UpdatedBy = adminId;
        EnsureSucceeded(await userManager.UpdateSecurityStampAsync(player), "rotating the player's security stamp");
        EnsureSucceeded(await userManager.UpdateAsync(player), "updating the player account");

        var outcome = suspend ? "PlayerSuspended" : "PlayerReactivated";
        await auditLogWriter.WriteAsync(
            "AdminAction",
            "Information",
            outcome,
            cancellationToken,
            actorUserId: adminId,
            actorUsername: User.Identity?.Name,
            targetType: "Player",
            targetId: player.Id.ToString("D"),
            details: new { Action = outcome, Reason = Input.Reason });
        await transaction.CommitAsync(cancellationToken);

        TempData["StatusMessage"] = suspend
            ? $"Player {player.UserName} was suspended."
            : $"Player {player.UserName} was reactivated.";
        return RedirectToPage(new { search = Search, status = Status, pageNumber = PageNumber });
    }

    private async Task LoadPlayersAsync(CancellationToken cancellationToken)
    {
        if (Status is not ("All" or "Active" or "Suspended"))
        {
            ModelState.AddModelError(nameof(Status), "Choose All, Active, or Suspended.");
            return;
        }

        var playerRoleId = await database.Roles
            .Where(role => role.Name == "Player")
            .Select(role => (Guid?)role.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (playerRoleId is null)
        {
            throw new InvalidOperationException("The Player role has not been initialized.");
        }

        var query = database.Users
            .AsNoTracking()
            .Where(user => database.UserRoles.Any(userRole =>
                userRole.UserId == user.Id && userRole.RoleId == playerRoleId.Value));
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            var userId = Guid.TryParse(search, out var parsedId) ? parsedId : (Guid?)null;
            query = query.Where(user =>
                user.UserName!.Contains(search)
                || (user.Email != null && user.Email.Contains(search))
                || (userId != null && user.Id == userId.Value));
        }

        if (Status == "Active")
        {
            query = query.Where(user => !user.IsSuspended);
        }
        else if (Status == "Suspended")
        {
            query = query.Where(user => user.IsSuspended);
        }

        TotalCount = await query.CountAsync(cancellationToken);
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Players = await query
            .OrderByDescending(user => user.CreatedAtUtc)
            .ThenBy(user => user.Id)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(user => new PlayerRow(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email,
                user.CreatedAtUtc,
                user.IsSuspended,
                user.SuspendedAtUtc,
                user.SuspensionReason))
            .ToListAsync(cancellationToken);
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Code));
            throw new InvalidOperationException($"Identity failed while {operation}: {errors}");
        }
    }

    public sealed record PlayerRow(
        Guid Id,
        string Username,
        string? Email,
        DateTimeOffset CreatedAtUtc,
        bool IsSuspended,
        DateTimeOffset? SuspendedAtUtc,
        string? SuspensionReason);

    public sealed class ModerationInput
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Reason { get; set; } = string.Empty;
    }
}
