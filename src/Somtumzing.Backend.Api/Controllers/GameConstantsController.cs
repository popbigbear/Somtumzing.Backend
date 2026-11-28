using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Controllers;

/// <summary>
/// Controller สำหรับจัดการ Game Constants (Dice, Speed Card, Delivery Tip, Immunity, etc.)
/// </summary>
[ApiController]
[Authorize(Roles = "Player")]
[Route("api/v1/game-constants")]
public sealed class GameConstantsController(
    GameDbContext database,
    ILogger<GameConstantsController> logger) : ControllerBase
{
    private static readonly EventId ConstantsLoadedEvent = new(2201, "GameConstantsLoaded");

    /// <summary>
    /// ดึง Game Constants ทั้งหมด แยกตาม Category
    /// GET /api/v1/game-constants
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GameConstantsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameConstantsResponse>> GetGameConstants(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var constants = await database.GameConstants
            .AsNoTracking()
            .Where(c => c.Status == "ACTIVE" && c.IsActive)
            .OrderBy(c => c.Category)
            .ThenBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var response = new GameConstantsResponse(
            DiceOutcomes: constants
                .Where(c => c.Category == "DICE")
                .Select(c => new GameConstantItem(
                    Key: c.ConstantKey,
                    Name: c.DisplayName,
                    Description: c.Description,
                    Value: c.ValueJson,
                    SortOrder: c.SortOrder
                )).ToList(),
            SpeedCards: constants
                .Where(c => c.Category == "SPEED_CARD")
                .Select(c => new GameConstantItem(
                    Key: c.ConstantKey,
                    Name: c.DisplayName,
                    Description: c.Description,
                    Value: c.ValueJson,
                    SortOrder: c.SortOrder
                )).ToList(),
            DeliveryTips: constants
                .Where(c => c.Category == "DELIVERY_TIP")
                .Select(c => new GameConstantItem(
                    Key: c.ConstantKey,
                    Name: c.DisplayName,
                    Description: c.Description,
                    Value: c.ValueJson,
                    SortOrder: c.SortOrder
                )).ToList(),
            Immunity: constants
                .Where(c => c.Category == "IMMUNITY")
                .Select(c => new GameConstantItem(
                    Key: c.ConstantKey,
                    Name: c.DisplayName,
                    Description: c.Description,
                    Value: c.ValueJson,
                    SortOrder: c.SortOrder
                )).ToList(),
            Other: constants
                .Where(c => c.Category != "DICE" && c.Category != "SPEED_CARD" && c.Category != "DELIVERY_TIP" && c.Category != "IMMUNITY")
                .Select(c => new GameConstantItem(
                    Key: c.ConstantKey,
                    Name: c.DisplayName,
                    Description: c.Description,
                    Value: c.ValueJson,
                    SortOrder: c.SortOrder
                )).ToList()
        );

        logger.LogInformation(ConstantsLoadedEvent, "Game constants loaded for player {PlayerId}: {Count} constants", userId, constants.Count);

        return Ok(response);
    }

    /// <summary>
    /// ดึง Game Constants ตาม Category
    /// GET /api/v1/game-constants/{category}
    /// </summary>
    [HttpGet("{category}")]
    [ProducesResponseType(typeof(List<GameConstantItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<GameConstantItem>>> GetGameConstantsByCategory(string category, CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var constants = await database.GameConstants
            .AsNoTracking()
            .Where(c => c.Status == "ACTIVE" && c.IsActive && c.Category.ToUpper() == category.ToUpper())
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var items = constants.Select(c => new GameConstantItem(
            Key: c.ConstantKey,
            Name: c.DisplayName,
            Description: c.Description,
            Value: c.ValueJson,
            SortOrder: c.SortOrder
        )).ToList();

        return Ok(items);
    }

    /// <summary>
    /// ดึง Game Constant เดียวตาม Key
    /// GET /api/v1/game-constants/key/{constantKey}
    /// </summary>
    [HttpGet("key/{constantKey}")]
    [ProducesResponseType(typeof(GameConstantItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameConstantItem>> GetGameConstantByKey(string constantKey, CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Unauthorized();
        if (user.IsSuspended) return ForbiddenProblem();

        var constant = await database.GameConstants
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.ConstantKey == constantKey && c.Status == "ACTIVE" && c.IsActive, cancellationToken);

        if (constant is null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Game constant '{constantKey}' not found."
            });
        }

        var item = new GameConstantItem(
            Key: constant.ConstantKey,
            Name: constant.DisplayName,
            Description: constant.Description,
            Value: constant.ValueJson,
            SortOrder: constant.SortOrder
        );

        return Ok(item);
    }

    private ObjectResult ForbiddenProblem() => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
    {
        Type = "https://tools.ietf.org/html/rfc7807",
        Title = "Forbidden",
        Status = StatusCodes.Status403Forbidden,
        Detail = "Account is suspended."
    });

    // DTO Records
    public sealed record GameConstantsResponse(
        List<GameConstantItem> DiceOutcomes,
        List<GameConstantItem> SpeedCards,
        List<GameConstantItem> DeliveryTips,
        List<GameConstantItem> Immunity,
        List<GameConstantItem> Other);

    public sealed record GameConstantItem(
        string Key,
        string Name,
        string? Description,
        string Value,
        int SortOrder);
}