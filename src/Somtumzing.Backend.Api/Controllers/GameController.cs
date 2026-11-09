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
    // เพิ่ม EventId สำหรับ Inventory ตามหลัก Clean Code และ Logging มาตรฐาน
    private static readonly EventId InventoryFetchedEvent = new(6001, "InventoryFetched");

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

        // ดึงข้อมูลผู้เล่นจากฐานข้อมูลแบบ Track เพื่อให้อัปเดตและบันทึกค่าจริงได้
        var user = await database.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return UnauthorizedProblem();
        if (user.IsSuspended) return ForbiddenProblem();

        // คำนวณเหรียญรางวัลและอัปเดตสถิติจริงลงในฐานข้อมูล
        int earnedCoins = request.Score / 10;
        user.Coins += earnedCoins;
        user.TotalServed += request.TotalServed;

        bool isNewBest = request.Score > user.BestScore;
        if (isNewBest)
        {
            user.BestScore = request.Score;
        }

        await database.SaveChangesAsync(cancellationToken);

        logger.LogInformation(GameEvent, "Player {PlayerId} finished run {RunId} with score {Score}, earned {Coins} coins", userId, runId, request.Score, earnedCoins);

        return Ok(new FinishRunResponse(
            RunId: runId,
            Score: request.Score,
            CoinsEarned: earnedCoins,
            TotalCoins: user.Coins, // ส่งคืนยอดเหรียญจริงล่าสุดจากฐานข้อมูล
            NewBestScore: isNewBest));
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

        // อ่านและส่งคืนข้อมูลจริงจาก Database
        return Ok(new FullPlayerResponse(
            Id: $"p_{user.Id:N}".Substring(0, 12),
            Username: user.UserName,
            DisplayName: user.DisplayName ?? user.UserName ?? "ผู้เล่นใหม่",
            Coins: user.Coins,             // อ่านค่าจริงจาก Database
            Energy: user.Energy,           // อ่านค่าจริงจาก Database
            BestScore: user.BestScore,     // อ่านค่าจริงจาก Database
            TotalServed: user.TotalServed, // อ่านค่าจริงจาก Database
            Upgrades: new Dictionary<string, int> { { "mortar_golden", 1 } },
            CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"));
    }

    // ==========================================
    // GET /api/v1/profile (Init Profile สำหรับเข้าเกม)
    // ==========================================
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfileResponse>> GetInitProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            int joinedDays = Math.Max(1, (int)(DateTimeOffset.UtcNow - user.CreatedAtUtc).TotalDays);

            var response = new ProfileResponse(
                Id: $"p_{user.Id:N}".Substring(0, 12),
                DisplayName: user.DisplayName ?? user.UserName ?? $"ร้านใหม่ #{user.Id.ToString("N").Substring(0, 4).ToUpper()}",
                Coins: user.Coins,         // อ่านค่า Coins จริงจาก Database
                BestScore: user.BestScore, // อ่านค่า BestScore จริงจาก Database
                TotalServed: user.TotalServed, // อ่านค่า TotalServed จริงจาก Database
                Upgrades: new Dictionary<string, int> { { "mortar_golden", 1 } }, // ปรับให้สอดคล้องกับ Endpoint อื่น
                CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                JoinedDays: joinedDays,
                RunsPlayed: 0);

            logger.LogInformation(GameEvent, "Init profile fetched successfully for player {PlayerId}", userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching player profile.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading profile.",
                Instance = HttpContext.Request.Path
            });
        }
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

     /// <summary>
    /// ดึงข้อมูลคลังของผู้เล่น (Energy, Ingredients, Cosmetics)
    /// GET /api/v1/inventory
    /// Security Level: Protected — Authorization: Bearer <token>
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(InventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventory(CancellationToken cancellationToken)
    {
        try
        {
            // 1. ตรวจสอบ Token และดึง User ID จาก Claims
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
            {
                return Unauthorized(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Token is expired or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            // 2. ตรวจสอบสถานะผู้เล่นในฐานข้อมูล
            var user = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Player not found.",
                    Instance = HttpContext.Request.Path
                });
            }

            // 3. ตรวจสอบสถานะบัญชีถูกระงับ (Suspended)
            if (user.IsSuspended)
            {
                logger.LogWarning("Suspended player {PlayerId} attempted to fetch inventory.", userId);
                return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Forbidden",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "Account is suspended.",
                    Instance = HttpContext.Request.Path
                });
            }

            // 4. ข้อมูล Inventory จำลองตาม Schema ที่กำหนด (สามารถดึงจาก Database จริงได้ในอนาคต)
            var ingredients = new Dictionary<string, int>
            {
                { "chili", 5 },
                { "lime", 5 },
                { "fish", 3 },
                { "sugar", 5 },
                { "fermented", 4 }
            };

            var cosmetics = new List<CosmeticItem>
            {
                new("cart-classic", "รถเข็นคู่ใจ", true),
                new("neon-sign", "ป้ายนีออน", false)
            };

            var response = new InventoryResponse(
                Energy: 10,
                MaxEnergy: 10,
                Ingredients: ingredients,
                Cosmetics: cosmetics);

            logger.LogInformation(InventoryFetchedEvent, "Inventory fetched successfully for player {PlayerId}", userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching player inventory.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading inventory.",
                Instance = HttpContext.Request.Path
            });
        }
    }

     public sealed record InventoryResponse(
        int Energy,
        int MaxEnergy,
        Dictionary<string, int> Ingredients,
        List<CosmeticItem> Cosmetics);

    public sealed record CosmeticItem(
        string Id,
        string Name,
        bool Unlocked);

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

    
    public sealed record ProfileResponse(
        string Id,
        string DisplayName,
        int Coins,
        int BestScore,
        int TotalServed,
        Dictionary<string, int> Upgrades,
        string CreatedAt,
        int JoinedDays,
        int RunsPlayed);
}