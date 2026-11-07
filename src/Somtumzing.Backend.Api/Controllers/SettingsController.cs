using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/settings")]
public sealed class SettingsController(
    GameDbContext database,
    ILogger<SettingsController> logger) : ControllerBase
{
    private static readonly EventId SettingsEvent = new(7001, "PlayerSettingsEvent");

    /// <summary>
    /// ดึงข้อมูลการตั้งค่าของผู้เล่นปัจจุบัน
    /// GET /api/v1/settings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SettingsResponse>> GetSettings(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // ค้นหา Setting ของผู้เล่น หากยังไม่มีให้สร้างค่าเริ่มต้น (Default)
            var settings = await database.PlayerSettings
                .SingleOrDefaultAsync(s => s.PlayerPK == userId, cancellationToken);

            if (settings is null)
            {
                settings = new PlayerSetting
                {
                    PK = Guid.NewGuid(),
                    PlayerPK = userId,
                    Sound = true,
                    Language = "th",
                    Analytics = true,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                };

                database.PlayerSettings.Add(settings);
                await database.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation(SettingsEvent, "Settings fetched for player {PlayerId}", userId);

            return Ok(new SettingsResponse(
                Sound: settings.Sound,
                Language: settings.Language,
                Analytics: settings.Analytics));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching player settings.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading settings.",
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

    /// <summary>
    /// อัปเดตการตั้งค่าของผู้เล่น (รองรับ Partial Update)
    /// PATCH /api/v1/settings
    /// </summary>
    [HttpPatch]
    [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings(
        [FromBody] UpdateSettingsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. ตรวจสอบความถูกต้องของ ModelState
            if (!ModelState.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Request parameters are invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // 2. ค้นหา Setting ของผู้เล่นในฐานข้อมูล หากยังไม่มีให้สร้างใหม่
            var settings = await database.PlayerSettings
                .SingleOrDefaultAsync(s => s.PlayerPK == userId, cancellationToken);

            if (settings is null)
            {
                settings = new PlayerSetting
                {
                    PK = Guid.NewGuid(),
                    PlayerPK = userId.Value,
                    Sound = true,
                    Language = "th",
                    Analytics = true,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                };
                database.PlayerSettings.Add(settings);
            }

            // 3. อัปเดตเฉพาะฟิลด์ที่มีการส่งค่าเข้ามา (Partial Update)
            if (request.Sound.HasValue)
            {
                settings.Sound = request.Sound.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                settings.Language = request.Language.Trim();
            }

            if (request.Analytics.HasValue)
            {
                settings.Analytics = request.Analytics.Value;
            }

            settings.UpdatedDate = DateTimeOffset.UtcNow;
            settings.UpdatedBy = userId;

            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation(SettingsEvent, "Settings updated successfully for player {PlayerId}", userId);

            return Ok(new SettingsResponse(
                Sound: settings.Sound,
                Language: settings.Language,
                Analytics: settings.Analytics));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating player settings.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while updating settings.",
                Instance = HttpContext.Request.Path
            });
        }
    }

    public sealed record SettingsResponse(
        bool Sound,
        string Language,
        bool Analytics);

     public sealed record UpdateSettingsRequest(
        bool? Sound,
        [StringLength(10, MinimumLength = 2)] string? Language,
        bool? Analytics);

}