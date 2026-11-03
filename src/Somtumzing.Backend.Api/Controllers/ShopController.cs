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

    [HttpGet("upgrades")]
    [ProducesResponseType(typeof(UpgradesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UpgradesResponse>> GetUpgrades(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Unauthorized();
        }

        if (user.IsSuspended)
        {
            logger.LogWarning("Suspended player {PlayerId} attempted to fetch shop upgrades.", userId);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Account is suspended." });
        }

        // รายการอัปเกรดจำลอง (สามารถดึงจาก Database แทนได้ในอนาคต)
        var upgrades = new List<UpgradeItem>
        {
            new(
                Id: "mortar_golden",
                Name: "ครกทองคำพิเศษ",
                Description: "เพิ่มความเร็วในการตำส้มตำขึ้น 25%",
                Cost: 500,
                Level: 1,
                MaxLevel: 5,
                IsUnlocked: true),
            new(
                Id: "pestle_titanium",
                Name: "สากไทเทเนียม",
                Description: "ลดเวลาคูลดาวน์การทำเมนูพิเศษลง 10%",
                Cost: 1200,
                Level: 0,
                MaxLevel: 3,
                IsUnlocked: false)
        };

        logger.LogInformation(UpgradesFetchedEvent, "Shop upgrades fetched successfully for player {PlayerId}", userId);

        return Ok(new UpgradesResponse(upgrades));
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