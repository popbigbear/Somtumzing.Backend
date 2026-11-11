using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/missions")]
public sealed class MissionsController(
    GameDbContext database,
    ILogger<MissionsController> logger) : ControllerBase
{
    private static readonly EventId MissionsEvent = new(8001, "PlayerMissionsEvent");

    /// <summary>
    /// ดึงรายการภารกิจและความคืบหน้าของผู้เล่นปัจจุบัน
    /// GET /api/v1/missions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(MissionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MissionsResponse>> GetMissions(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // 1. ดึงรายการภารกิจจากฐานข้อมูลของผู้เล่น
            var playerMissions = await database.PlayerMissions
                .Where(m => m.PlayerPK == userId.Value)
                .ToListAsync(cancellationToken);

            // 2. หากผู้เล่นยังไม่มีข้อมูลภารกิจเริ่มต้น ให้สร้าง Mission พื้นฐานให้ (เช่น "serve-10")
            if (playerMissions.Count == 0)
            {
                var initialMission = new PlayerMission
                {
                    PK = Guid.NewGuid(),
                    PlayerPK = userId.Value,
                    MissionKey = "serve-10",
                    Progress = 0,
                    TargetValue = 10,
                    IsClaimed = false,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Status = "ACTIVE"
                };

                database.PlayerMissions.Add(initialMission);
                await database.SaveChangesAsync(cancellationToken);
                playerMissions.Add(initialMission);
            }

            // 3. แมปข้อมูลส่งกลับตาม Response Schema ที่กำหนด
            var missionItems = playerMissions.Select(m => new MissionItem(
                Id: m.MissionKey,
                Title: GetMissionTitle(m.MissionKey),
                Description: GetMissionDescription(m.MissionKey),
                Progress: m.Progress,
                Target: m.TargetValue,
                RewardCoins: 75,
                Claimed: m.IsClaimed
            )).ToList();

            logger.LogInformation(MissionsEvent, "Missions fetched successfully for player {PlayerId}", userId);

            return Ok(new MissionsResponse(missionItems));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching player missions.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading missions.",
                Instance = HttpContext.Request.Path
            });
        }
    }

    private static string GetMissionTitle(string missionKey) => missionKey switch
    {
        "serve-10" => "ตำให้ครบสิบ",
        _ => "ภารกิจพิเศษ"
    };

    private static string GetMissionDescription(string missionKey) => missionKey switch
    {
        "serve-10" => "เสิร์ฟออเดอร์ให้ครบ 10 ครก",
        _ => "ทำภารกิจในเกมให้สำเร็จเพื่อรับรางวัล"
    };

    // ==========================================
    // POST /api/v1/missions/{missionId}/claim (Claim Mission Reward)
    // ==========================================
    [HttpPost("{missionId}/claim")]
    [ProducesResponseType(typeof(ClaimMissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClaimMissionResponse>> ClaimMissionReward(
        string missionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // 1. ค้นหาภารกิจของผู้เล่น (หากไม่พบ ส่ง 404 Not Found พร้อมรหัส NOT_FOUND)
            var mission = await database.PlayerMissions
                .SingleOrDefaultAsync(m => m.PlayerPK == userId.Value && m.MissionKey == missionId, cancellationToken);

            if (mission is null)
            {
                return NotFound(new ApiErrorResponse(
                    Code: "NOT_FOUND",
                    Message: "ไม่พบภารกิจ"));
            }

            // 2. ตรวจสอบว่าภารกิจทำสำเร็จหรือยัง (หากยังไม่ครบ ส่ง 409 Conflict พร้อมรหัส VALIDATION_FAILED)
            if (mission.Progress < mission.TargetValue)
            {
                return Conflict(new ApiErrorResponse(
                    Code: "VALIDATION_FAILED",
                    Message: "ยังทำภารกิจไม่ครบ"));
            }

            // 3. ตรวจสอบว่าเคยกดรับรางวัลไปแล้วหรือยัง (หากรับซ้ำ ส่ง 409 Conflict พร้อมรหัส MISSION_ALREADY_CLAIMED)
            if (mission.IsClaimed)
            {
                return Conflict(new ApiErrorResponse(
                    Code: "MISSION_ALREADY_CLAIMED",
                    Message: "รับรางวัลภารกิจไปแล้ว"));
            }

            // 4. ทำการอัปเดตสถานะ Claimed และบันทึกข้อมูล
            const int rewardCoins = 75;
            mission.IsClaimed = true;
            mission.UpdatedDate = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation(MissionsEvent, "Player {PlayerId} claimed reward for mission {MissionId}", userId, missionId);

            // 5. ส่งคืน Response ตาม Schema ที่กำหนด
            var updatedPlayer = new PlayerProfileResponse(
                Id: $"p_{user.Id:N}".Substring(0, 12),
                DisplayName: user.DisplayName ?? user.UserName ?? $"ร้านใหม่ #{user.Id.ToString("N").Substring(0, 4).ToUpper()}",
                Coins: 100 + rewardCoins,
                BestScore: 0,
                TotalServed: mission.Progress,
                Upgrades: new Dictionary<string, int>(),
                CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                JoinedDays: Math.Max(1, (int)(DateTimeOffset.UtcNow - user.CreatedAtUtc).TotalDays),
                RunsPlayed: 0);

            return Ok(new ClaimMissionResponse(
                Player: updatedPlayer,
                RewardCoins: rewardCoins));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while claiming mission reward.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while claiming reward.",
                Instance = HttpContext.Request.Path
            });
        }
    }

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

    public sealed record MissionsResponse(List<MissionItem> Missions);

    public sealed record MissionItem(
        string Id,
        string Title,
        string Description,
        int Progress,
        int Target,
        int RewardCoins,
        bool Claimed);

    public sealed record ApiErrorResponse(
        string Code,
        string Message);

    public sealed record ClaimMissionResponse(
        PlayerProfileResponse Player,
        int RewardCoins);

    public sealed record PlayerProfileResponse(
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