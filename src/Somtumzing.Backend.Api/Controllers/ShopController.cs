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

            var user = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // มืออาชีพ: ใช้ Left Join ระหว่าง Upgrades (Master) และ PlayerUpgrades (ผู้เล่น)
            // เพื่อให้ผู้เล่นเห็นรายการอัปเกรดทั้งหมดในเกม แม้จะยังไม่เคยซื้อเลยก็ตาม (Level = 0)
            var query = from u in database.Upgrades.AsNoTracking()
                        join pu in database.PlayerUpgrades.AsNoTracking()
                            on new { UpgradePK = u.PK, PlayerPK = userId } equals new { pu.UpgradePK, pu.PlayerPK } into playerUpgrades
                        from pu in playerUpgrades.DefaultIfEmpty() // Left Join
                        where u.Status == "PUBLISHED"
                        select new UpgradeItem(
                            Id: u.UpgradeKey,
                            Name: u.DisplayName,
                            Description: u.Description ?? "อัปเกรดเพิ่มความสามารถร้านค้า",
                            Cost: (int)u.Price * (pu != null ? pu.Level + 1 : 1),
                            Level: pu != null ? pu.Level : 0,
                            MaxLevel: u.MaxLevel,
                            IsUnlocked: pu != null && pu.Level > 0
                        );

            var upgrades = await query.ToListAsync(cancellationToken);

            logger.LogInformation(UpgradesFetchedEvent, "Shop upgrades fetched successfully from database for player {PlayerId}", userId);

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

    // เพิ่ม Helper Methods สำหรับ Error Responses ตามมาตรฐาน RFC 7807
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