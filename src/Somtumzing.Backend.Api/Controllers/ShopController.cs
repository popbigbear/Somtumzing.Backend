using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/shop")]
public sealed class ShopController(
    GameDbContext database,
    ILogger<ShopController> logger) : ControllerBase
{
    private static readonly EventId UpgradesFetchedEvent = new(3001, "ShopUpgradesFetched");

     /// <summary>
    /// ดึงรายการอัปเกรดร้านค้าของผู้เล่น
    /// GET /api/v1/shop/upgrades
    /// Security Level: Protected — Authorization: Bearer <token>
    /// </summary>
    [HttpGet("upgrades")]
    [ProducesResponseType(typeof(UpgradesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpgradesResponse>> GetUpgrades(CancellationToken cancellationToken)
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
                logger.LogWarning("Suspended player {PlayerId} attempted to fetch shop upgrades.", userId);
                return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Forbidden",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "Account is suspended.",
                    Instance = HttpContext.Request.Path
                });
            }

            // 4. รายการอัปเกรดจำลองตาม Schema ที่กำหนด
            var upgrades = new List<UpgradeItem>
            {
                new(
                    Id: "mortar_golden",
                    Name: "ครกทองคำพิเศษ",
                    Description: "เพิ่มความเร็วในการตำส้มตำขึ้น 25%",
                    Cost: 150,
                    Level: 1,
                    MaxLevel: 5,
                    IsUnlocked: true),
                new(
                    Id: "pestle_titanium",
                    Name: "สากไทเทเนียม",
                    Description: "ลดเวลาคูลดาวน์การทำเมนูพิเศษลง 10%",
                    Cost: 300,
                    Level: 0,
                    MaxLevel: 3,
                    IsUnlocked: false)
            };

            logger.LogInformation(UpgradesFetchedEvent, "Shop upgrades fetched successfully for player {PlayerId}", userId);

            return Ok(new UpgradesResponse(upgrades));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching shop upgrades.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading shop upgrades.",
                Instance = HttpContext.Request.Path
            });
        }
    }

    public sealed record UpgradesResponse(List<UpgradeItem> Upgrades);

    public sealed record UpgradeItem(
        string Id,
        string Name,
        string Description,
        int Cost,
        int Level,
        int MaxLevel,
        bool IsUnlocked);
}