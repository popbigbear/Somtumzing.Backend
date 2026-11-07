using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1")]
public sealed class GameController(
    GameDbContext database,
    ILogger<GameController> logger) : ControllerBase
{
    private static readonly EventId GameEvent = new(5001, "GameCoreEvent");

    // ==========================================
    // 1. POST /api/v1/runs (Start Run)
    // ==========================================
    [HttpPost("runs")]
    [ProducesResponseType(typeof(RunResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RunResponse>> StartRun(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return UnauthorizedProblem();

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return UnauthorizedProblem();
        if (user.IsSuspended) return ForbiddenProblem();

        // จำลองการสร้าง Run ใหม่ในเกม
        var runId = $"run_{Guid.NewGuid():N}".Substring(0, 16);
        logger.LogInformation(GameEvent, "Player {PlayerId} started a new run {RunId}", userId, runId);

        return Ok(new RunResponse(
            RunId: runId,
            Status: "IN_PROGRESS",
            StartedAt: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"));
    }

    // ==========================================
    // 2. POST /api/v1/runs/{runId}/finish (Finish Run)
    // ==========================================
    [HttpPost("runs/{runId}/finish")]
    [ProducesResponseType(typeof(FinishRunResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FinishRunResponse>> FinishRun(
        string runId,
        [FromBody] FinishRunRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return UnauthorizedProblem();

        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Invalid score or metrics provided."
            });
        }

        var user = await database.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return UnauthorizedProblem();
        if (user.IsSuspended) return ForbiddenProblem();

        // คำนวณรางวัลและอัปเดตสถิติผู้เล่น (จำลอง)
        int earnedCoins = request.Score / 10;
        logger.LogInformation(GameEvent, "Player {PlayerId} finished run {RunId} with score {Score}", userId, runId, request.Score);

        return Ok(new FinishRunResponse(
            RunId: runId,
            Score: request.Score,
            CoinsEarned: earnedCoins,
            TotalCoins: 100 + earnedCoins,
            NewBestScore: request.Score > 0));
    }

    // ==========================================
    // 3. GET /api/v1/leaderboard (Leaderboard)
    // ==========================================
    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeaderboardResponse>> GetLeaderboard(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return UnauthorizedProblem();

        // จำลองข้อมูล Leaderboard อันดับผู้เล่น
        var leaders = new List<LeaderboardItem>
        {
            new(Rank: 1, PlayerId: "p_proplayer01", DisplayName: "เซียนส้มตำ", BestScore: 5420),
            new(Rank: 2, PlayerId: "p_somtummaster", DisplayName: "ครกทองคำ", BestScore: 4890),
            new(Rank: 3, PlayerId: "p_papayaking", DisplayName: "ราชาละมุด", BestScore: 4120)
        };

        return Ok(new LeaderboardResponse(leaders));
    }

    // ==========================================
    // 4. POST /api/v1/shop/upgrades/{upgradeId}/purchase (Purchase Upgrade)
    // ==========================================
    [HttpPost("shop/upgrades/{upgradeId}/purchase")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PurchaseResponse>> PurchaseUpgrade(
        string upgradeId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return UnauthorizedProblem();

        var user = await database.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return UnauthorizedProblem();
        if (user.IsSuspended) return ForbiddenProblem();

        // ตรวจสอบไอเทมและเหรียญของผู้เล่น (จำลองการซื้อสำเร็จ)
        logger.LogInformation(GameEvent, "Player {PlayerId} purchased upgrade {UpgradeId}", userId, upgradeId);

        return Ok(new PurchaseResponse(
            UpgradeId: upgradeId,
            NewLevel: 1,
            RemainingCoins: 350));
    }

    // ==========================================
    // 5. GET /api/v1/players/me (Full Player Status)
    // ==========================================
    [HttpGet("players/me")]
    [ProducesResponseType(typeof(FullPlayerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FullPlayerResponse>> GetFullPlayerProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return UnauthorizedProblem();

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return UnauthorizedProblem();
        if (user.IsSuspended) return ForbiddenProblem();

        return Ok(new FullPlayerResponse(
            Id: $"p_{user.Id:N}".Substring(0, 12),
            Username: user.UserName,
            DisplayName: user.DisplayName ?? user.UserName ?? "ผู้เล่นใหม่",
            Coins: 500,
            Energy: 10,
            BestScore: 1250,
            TotalServed: 45,
            Upgrades: new Dictionary<string, int> { { "mortar_golden", 1 } },
            CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"));
    }

    // Helper Methods สำหรับ RFC 7807 Errors
    private Guid? GetCurrentUserId()
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(subject, out var userId) ? userId : null;
    }

    private UnauthorizedObjectResult UnauthorizedProblem() => Unauthorized(new ProblemDetails
    {
        Type = "https://tools.ietf.org/html/rfc7807",
        Title = "Unauthorized",
        Status = StatusCodes.Status401Unauthorized,
        Detail = "Token is expired or invalid."
    });

    private ObjectResult ForbiddenProblem() => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
    {
        Type = "https://tools.ietf.org/html/rfc7807",
        Title = "Forbidden",
        Status = StatusCodes.Status403Forbidden,
        Detail = "Account is suspended."
    });

    // Records สำหรับ Request/Response
    public sealed record RunResponse(string RunId, string Status, string StartedAt);
    public sealed record FinishRunRequest([Range(0, int.MaxValue)] int Score, int TotalServed);
    public sealed record FinishRunResponse(string RunId, int Score, int CoinsEarned, int TotalCoins, bool NewBestScore);
    public sealed record LeaderboardItem(int Rank, string PlayerId, string DisplayName, int BestScore);
    public sealed record LeaderboardResponse(List<LeaderboardItem> Leaders);
    public sealed record PurchaseResponse(string UpgradeId, int NewLevel, int RemainingCoins);
    public sealed record FullPlayerResponse(
        string Id,
        string? Username,
        string DisplayName,
        int Coins,
        int Energy,
        int BestScore,
        int TotalServed,
        Dictionary<string, int> Upgrades,
        string CreatedAt);
}