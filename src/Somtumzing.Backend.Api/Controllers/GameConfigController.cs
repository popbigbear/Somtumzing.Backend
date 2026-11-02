using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Controllers;

/// <summary>
/// Controller สำหรับจัดการและให้บริการข้อมูลการตั้งค่าเกม (Game Configuration)
/// ทุก Endpoint ต้องผ่านการยืนยันตัวตนระดับ Protected (Bearer Token)
/// </summary>
[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/config")]
public sealed class GameConfigController(
    GameDbContext database,
    ILogger<GameConfigController> logger) : ControllerBase
{
    private static readonly EventId ConfigLoadedEvent = new(2001, "GameConfigLoaded");

    /// <summary>
    /// ดึงข้อมูลการตั้งค่าเกมปัจจุบันสำหรับผู้เล่น
    /// GET /api/v1/config
    /// Security Level: Protected — Authorization: Bearer <token>
    /// </summary>
    /// <param name="cancellationToken">Cancellation token สำหรับยกเลิก request</param>
    /// <returns>GameConfigResponse ที่มีค่าคอนฟิกของเกม</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GameConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GameConfigResponse>> GetConfig(CancellationToken cancellationToken)
    {
        // ตรวจสอบ Claim ระบุตัวตนของผู้เล่น (sub หรือ NameIdentifier)
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        // ตรวจสอบสถานะบัญชีผู้เล่นว่าถูกระงับหรือไม่
        var user = await database.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Unauthorized();
        }

        if (user.IsSuspended)
        {
            logger.LogWarning("Suspended player attempted to fetch game config: {PlayerId}", userId);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Account is suspended." });
        }

        // คืนค่า Game Configuration ที่สมบูรณ์พร้อมใช้งาน
        var config = new GameConfigResponse(
            Version: "1.0.0",
            MaxEnergy: 100,
            EnergyRegenSeconds: 300,
            DailyRewardEnabled: true,
            MaintenanceMode: false,
            SupportedPlatforms: ["web", "ios", "android"],
            ServerTimeUtc: DateTimeOffset.UtcNow);

        logger.LogInformation(
            ConfigLoadedEvent,
            "Game config loaded successfully for player {PlayerId}",
            userId);

        return Ok(config);
    }

    /// <summary>
    /// ข้อมูล Game Configuration ส่งกลับให้ Client
    /// </summary>
    public sealed record GameConfigResponse(
        string Version,
        int MaxEnergy,
        int EnergyRegenSeconds,
        bool DailyRewardEnabled,
        bool MaintenanceMode,
        string[] SupportedPlatforms,
        DateTimeOffset ServerTimeUtc);
}