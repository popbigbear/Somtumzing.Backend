using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

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
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Account is suspended."
            });
        }

        // โหลด Game Configuration จากฐานข้อมูล
        var dbConfig = await database.GameConfigs
            .AsNoTracking()
            .Where(c => c.Status == "ACTIVE")
            .OrderByDescending(c => c.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        // Parse tables JSON
        var tables = new List<TableConfig>();
        if (dbConfig != null && !string.IsNullOrWhiteSpace(dbConfig.TablesJson) && dbConfig.TablesJson != "[]")
        {
            try
            {
                tables = JsonSerializer.Deserialize<List<TableConfig>>(dbConfig.TablesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TableConfig>();
            }
            catch
            {
                tables = new List<TableConfig>();
            }
        }

        // Default tables if none configured
        if (tables.Count == 0)
        {
            tables = new List<TableConfig>
            {
                new(1, "โต๊ะ 1", "ป้าแดง", "urgent", 1),
                new(2, "โต๊ะ 2", "พี่วินหน้าซอย", "waiting", 2),
                new(3, "โต๊ะ 3", "น้องออฟฟิศ", "waiting", 1),
                new(4, "โต๊ะ 4", "เจ๊ขายหวย", "waiting", 3)
            };
        }

        // Parse dice rounds JSON
        var diceRounds = new List<DiceRoundConfig>();
        if (dbConfig != null && !string.IsNullOrWhiteSpace(dbConfig.DiceRoundsJson) && dbConfig.DiceRoundsJson != "[]")
        {
            try
            {
                diceRounds = JsonSerializer.Deserialize<List<DiceRoundConfig>>(dbConfig.DiceRoundsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<DiceRoundConfig>();
            }
            catch
            {
                diceRounds = new List<DiceRoundConfig>();
            }
        }

        // Default dice rounds if none configured
        if (diceRounds.Count == 0)
        {
            diceRounds = new List<DiceRoundConfig>
            {
                new("เช้า", null, new int[0], null, null),
                new("กลางวัน", null, new int[0], null, null),
                new("เย็น", null, new int[0], null, null)
            };
        }

        // ถ้ายังไม่มี config ให้ใช้ค่าเริ่มต้น
        var config = new GameConfigResponse(
            Version: dbConfig?.Version ?? "1.0.0",
            MaxEnergy: dbConfig?.MaxEnergy ?? 10,
            EnergyRegenSeconds: dbConfig?.EnergyRegenSeconds ?? 300,
            DailyRewardEnabled: dbConfig?.DailyRewardEnabled ?? true,
            MaintenanceMode: dbConfig?.MaintenanceMode ?? false,
            SupportedPlatforms: dbConfig?.SupportedPlatforms ?? ["web", "mobile"],
            ServerTimeUtc: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z",
            Customers: dbConfig?.Customers ?? Array.Empty<string>(),
            RunSeconds: dbConfig?.RunSeconds ?? 180,
            MaxPerIngredient: dbConfig?.MaxPerIngredient ?? 10,
            MaxWarnings: dbConfig?.MaxWarnings ?? 3,
            PaotungScansRequired: dbConfig?.PaotungScansRequired ?? 3,
            TessakitFine: dbConfig?.TessakitFine ?? 50,
            PaotungChance: dbConfig?.PaotungChance ?? 20,
            PaotungBonus: dbConfig?.PaotungBonus ?? 100,
            PaotungFailChance: dbConfig?.PaotungFailChance ?? 10,
            TessakitChancePerSec: dbConfig?.TessakitChancePerSec ?? 5,
            TessakitWarningSec: dbConfig?.TessakitWarningSec ?? 10,
            TableCount: dbConfig?.TableCount ?? 4,
            DeliveryOrders: dbConfig?.DeliveryOrders ?? 2,
            MaxPowerCards: dbConfig?.MaxPowerCards ?? 2,
            DiceWinCoins: dbConfig?.DiceWinCoins ?? 25,
            DiceLossCoins: dbConfig?.DiceLossCoins ?? -10,
            DiceWinPowerCard: dbConfig?.DiceWinPowerCard ?? true,
            SpeedCardDurationSec: dbConfig?.SpeedCardDurationSec ?? 12,
            SpeedCardBonusCoins: dbConfig?.SpeedCardBonusCoins ?? 10,
            DeliveryTipCoins: dbConfig?.DeliveryTipCoins ?? 15,
            FleeImmunitySec: dbConfig?.FleeImmunitySec ?? 6,
            QueueCapacity: dbConfig?.QueueCapacity ?? 5,
            UrgentThreshold: dbConfig?.UrgentThreshold ?? 4,
            WarningTimeSec: dbConfig?.WarningTimeSec ?? 10,
            UrgentSlotBase: dbConfig?.UrgentSlotBase ?? 1,
            UrgentSlotIntervalSec: dbConfig?.UrgentSlotIntervalSec ?? 10,
            Tables: tables,
            DiceRounds: diceRounds);

         logger.LogInformation(
            ConfigLoadedEvent,
            "Game config loaded successfully for player {PlayerId}",
            userId);

        return Ok(config);
    }

    /// <summary>
    /// Initial table configuration for run screen
    /// </summary>
    public sealed record TableConfig(
        int Id,
        string Label,
        string Customer,
        string Status, // waiting, served, urgent
        int Orders);

    /// <summary>
    /// Dice round configuration for run screen
    /// </summary>
    public sealed record DiceRoundConfig(
        string Label,
        string? Choice, // "high" | "low" | null
        int[] Dice,
        int? Total,
        int? Delta);

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
        string ServerTimeUtc,
        string[] Customers,
        int RunSeconds,
        int MaxPerIngredient,
        int MaxWarnings,
        int PaotungScansRequired,
        int TessakitFine,
        int PaotungChance,
        int PaotungBonus,
        int PaotungFailChance,
        int TessakitChancePerSec,
        int TessakitWarningSec,
        int TableCount,
        int DeliveryOrders,
        int MaxPowerCards,
        int DiceWinCoins,
        int DiceLossCoins,
        bool DiceWinPowerCard,
        int SpeedCardDurationSec,
        int SpeedCardBonusCoins,
        int DeliveryTipCoins,
        int FleeImmunitySec,
        int QueueCapacity,
        int UrgentThreshold,
        int WarningTimeSec,
        int UrgentSlotBase,
        int UrgentSlotIntervalSec,
        List<TableConfig> Tables,
        List<DiceRoundConfig> DiceRounds);
}