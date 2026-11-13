using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using System.Data.Common; // <-- เพิ่ม namespace นี้


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
    
    // เพิ่ม EventId สำหรับ Mission ตามมาตรฐาน Logging
    private static readonly EventId MissionEvent = new(7001, "MissionCoreEvent");

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
    /// ดึงข้อมูลคลังของผู้เล่น (Energy, Ingredients, Cosmetics) จาก Database จริง
    /// GET /api/v1/inventory
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
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedProblem();

            var user = await database.Users
                .SingleOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            // 1. ดึงข้อมูลส่วนผสม (Ingredients) จาก Database ของผู้เล่น
            var playerIngredients = await database.PlayerIngredients
                .Where(i => i.PlayerPK == userId.Value)
                .ToListAsync(cancellationToken);

            // หากยังไม่มีข้อมูลเริ่มต้น ให้สร้างชุดเริ่มต้นให้
            if (playerIngredients.Count == 0)
            {
                var defaultIngredients = new List<PlayerIngredient>
                {
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, ItemKey = "chili", Quantity = 5 },
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, ItemKey = "lime", Quantity = 5 },
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, ItemKey = "fish", Quantity = 3 },
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, ItemKey = "sugar", Quantity = 5 },
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, ItemKey = "fermented", Quantity = 4 }
                };
                database.PlayerIngredients.AddRange(defaultIngredients);
                await database.SaveChangesAsync(cancellationToken);
                playerIngredients = defaultIngredients;
            }

            // 2. ดึงข้อมูลคอสเมติกส์ (Cosmetics) จาก Database ของผู้เล่น
            var playerCosmetics = await database.PlayerCosmetics
                .Where(c => c.PlayerPK == userId.Value)
                .ToListAsync(cancellationToken);

            if (playerCosmetics.Count == 0)
            {
                var defaultCosmetics = new List<PlayerCosmetic>
                {
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, CosmeticKey = "cart-classic", DisplayName = "รถเข็นคู่ใจ", Unlocked = true },
                    new() { PK = Guid.NewGuid(), PlayerPK = userId.Value, CosmeticKey = "neon-sign", DisplayName = "ป้ายนีออน", Unlocked = false }
                };
                database.PlayerCosmetics.AddRange(defaultCosmetics);
                await database.SaveChangesAsync(cancellationToken);
                playerCosmetics = defaultCosmetics;
            }

            var ingredientsDict = playerIngredients.ToDictionary(i => i.ItemKey, i => i.Quantity);
            var cosmeticsList = playerCosmetics.Select(c => new CosmeticItem(c.CosmeticKey, c.DisplayName, c.Unlocked)).ToList();

            var response = new InventoryResponse(
                Energy: user.Energy,   // ดึงจากฟิลด์ Energy จริงในตาราง Users
                MaxEnergy: 10,
                Ingredients: ingredientsDict,
                Cosmetics: cosmeticsList);

            logger.LogInformation(InventoryFetchedEvent, "Inventory fetched successfully from DB for player {PlayerId}", userId);

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

    // // ==========================================
    // // POST /api/v1/missions/{missionId}/claim (เคลมรางวัลภารกิจ)
    // // ==========================================
    // [HttpPost("missions/{missionId}/claim")]
    // [ProducesResponseType(typeof(ClaimMissionResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status409Conflict)]
    // public async Task<ActionResult<ClaimMissionResponse>> ClaimMission(
    //     string missionId,
    //     CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var userId = GetCurrentUserId();
    //         if (userId == null) return UnauthorizedProblem();

    //         var user = await database.Users
    //             .SingleOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

    //         if (user is null) return UnauthorizedProblem();
    //         if (user.IsSuspended) return ForbiddenProblem();

    //         // ค้นหาภารกิจของผู้เล่น
    //         var mission = await database.PlayerMissions
    //             .SingleOrDefaultAsync(m => m.PlayerPK == userId.Value && m.MissionKey == missionId, cancellationToken);

    //         if (mission is null)
    //         {
    //             return NotFound(new ProblemDetails
    //             {
    //                 Type = "https://tools.ietf.org/html/rfc7807",
    //                 Title = "Not Found",
    //                 Status = StatusCodes.Status404NotFound,
    //                 Detail = "Mission not found."
    //             });
    //         }

    //         // ตรวจสอบว่าภารกิจทำสำเร็จหรือยัง และเคลมไปแล้วหรือยัง
    //         if (!mission.IsCompleted)
    //         {
    //             return BadRequest(new ProblemDetails
    //             {
    //                 Type = "https://tools.ietf.org/html/rfc7807",
    //                 Title = "Bad Request",
    //                 Status = StatusCodes.Status400BadRequest,
    //                 Detail = "Mission is not yet completed."
    //             });
    //         }

    //         if (mission.IsClaimed)
    //         {
    //             return Conflict(new ProblemDetails
    //             {
    //                 Type = "https://tools.ietf.org/html/rfc7807",
    //                 Title = "Conflict",
    //                 Status = StatusCodes.Status409Conflict,
    //                 Detail = "Mission reward has already been claimed."
    //             });
    //         }

    //         // ให้รางวัลเหรียญ (Coins) และอัปเดตสถานะภารกิจว่าเคลมแล้ว
    //         int rewardCoins = 50;
    //         user.Coins += rewardCoins;
    //         mission.IsClaimed = true;
    //         mission.UpdatedDate = DateTimeOffset.UtcNow;

    //         await database.SaveChangesAsync(cancellationToken);

    //         logger.LogInformation(MissionEvent, "Mission {MissionId} claimed successfully by player {PlayerId}, earned {Coins} coins", missionId, userId, rewardCoins);

    //         return Ok(new ClaimMissionResponse(
    //             MissionId: missionId,
    //             CoinsAwarded: rewardCoins,
    //             TotalCoins: user.Coins));
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError(ex, "An error occurred while claiming mission reward.");
    //         return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
    //         {
    //             Type = "https://tools.ietf.org/html/rfc7807",
    //             Title = "Backend Error",
    //             Status = StatusCodes.Status500InternalServerError,
    //             Detail = "An unexpected error occurred while claiming mission.",
    //             Instance = HttpContext.Request.Path
    //         });
    //     }
    // }

     // ตัวอย่างการใช้ ADO.NET ดึงข้อมูล Missions ร่วมกับการ JOIN
    private async Task<List<MissionItem>> GetMissionsViaAdoNetAsync(Guid playerPk, CancellationToken cancellationToken)
    {
        var missions = new List<MissionItem>();
        var connection = database.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT pm.""MissionKey"", 
                   COALESCE(m.""Title"", 'ภารกิจพิเศษ') AS ""Title"", 
                   COALESCE(m.""Description"", 'ทำภารกิจเพื่อรับรางวัล') AS ""Description"", 
                   pm.""Progress"", 
                   pm.""TargetValue"", 
                   COALESCE(m.""RewardCoins"", 50) AS ""RewardCoins"", 
                   pm.""IsCompleted"", 
                   pm.""IsClaimed""
            FROM public.""PlayerMissions"" pm
            LEFT JOIN public.""Missions"" m ON pm.""MissionKey"" = m.""MissionKey""
            WHERE pm.""PlayerPK"" = @PlayerPK;
        ";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@PlayerPK";
        parameter.Value = playerPk;
        command.Parameters.Add(parameter);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            missions.Add(new MissionItem(
                Id: reader.GetString(0),
                Title: reader.GetString(1),
                Description: reader.GetString(2),
                Progress: reader.GetInt32(3),
                Target: reader.GetInt32(4),
                RewardCoins: reader.GetInt32(5),
                IsCompleted: reader.GetBoolean(6),
                IsClaimed: reader.GetBoolean(7)
            ));
        }

        return missions;
    }


    // [HttpGet("missions")]
    // [ProducesResponseType(typeof(MissionsResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public async Task<ActionResult<MissionsResponse>> GetMissions(CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var userId = GetCurrentUserId();
    //         if (userId == null) return UnauthorizedProblem();

    //         var user = await database.Users
    //             .AsNoTracking()
    //             .SingleOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

    //         if (user is null) return UnauthorizedProblem();
    //         if (user.IsSuspended) return ForbiddenProblem();

    //         // EF Core: JOIN ตาราง PlayerMissions กับ Master Table (Missions)
    //         var query = from pm in database.PlayerMissions.AsNoTracking()
    //                     join m in database.Missions.AsNoTracking() on pm.MissionKey equals m.MissionKey into missionDetails
    //                     from m in missionDetails.DefaultIfEmpty()
    //                     where pm.PlayerPK == userId.Value
    //                     select new MissionItem(
    //                         Id: pm.MissionKey,
    //                         Title: m != null ? m.Title : "ภารกิจพิเศษ",
    //                         Description: m != null ? m.Description : "ทำภารกิจในเกมให้สำเร็จเพื่อรับรางวัล",
    //                         Progress: pm.Progress,
    //                         Target: pm.TargetValue,
    //                         RewardCoins: m != null ? m.RewardCoins : 50,
    //                         IsCompleted: pm.IsCompleted,
    //                         IsClaimed: pm.IsClaimed
    //                     );

    //         var missions = await query.ToListAsync(cancellationToken);

    //         logger.LogInformation(MissionEvent, "Missions fetched successfully via EF Core for player {PlayerId}", userId);

    //         return Ok(new MissionsResponse(missions));
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError(ex, "An error occurred while fetching player missions.");
    //         return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
    //         {
    //             Type = "https://tools.ietf.org/html/rfc7807",
    //             Title = "Backend Error",
    //             Status = StatusCodes.Status500InternalServerError,
    //             Detail = "An unexpected error occurred while loading missions.",
    //             Instance = HttpContext.Request.Path
    //         });
    //     }
    // }

     // ==========================================
    // GET /api/v1/daily-rewards (ดึงสถานะรางวัลประจำวัน)
    // ==========================================
    [HttpGet("daily-rewards")]
    [ProducesResponseType(typeof(DailyRewardsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DailyRewardsResponse>> GetDailyRewards(CancellationToken cancellationToken)
    {
        try
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(subject, out var playerPk)) return UnauthorizedProblem();

            var user = await database.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == playerPk, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            var dailyState = await database.PlayerDailyStates
                .SingleOrDefaultAsync(d => d.PlayerPK == playerPk, cancellationToken);

            var today = DateTimeOffset.UtcNow.Date;

            if (dailyState is null)
            {
                dailyState = new PlayerDailyState
                {
                    PK = Guid.NewGuid(),
                    Code = $"DLY_{Guid.NewGuid():N}".Substring(0, 16),
                    Status = "ACTIVE",
                    PlayerPK = playerPk,
                    CurrentStreak = 0,
                    TodayClaimed = false,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                };
                database.PlayerDailyStates.Add(dailyState);
                await database.SaveChangesAsync(cancellationToken);
            }
            else
            {
                // ตรวจสอบว่าข้ามวันหรือไม่ หากข้ามวันและไม่ได้กดรับเมื่อวาน ให้รีเซ็ต Streak หรือจัดการตามรอบวัน
                if (dailyState.LastClaimedDate.HasValue && dailyState.LastClaimedDate.Value.Date < today.AddDays(-1))
                {
                    dailyState.CurrentStreak = 0;
                    dailyState.TodayClaimed = false;
                    dailyState.UpdatedDate = DateTimeOffset.UtcNow;
                    await database.SaveChangesAsync(cancellationToken);
                }
                else if (dailyState.LastClaimedDate.HasValue && dailyState.LastClaimedDate.Value.Date == today)
                {
                    dailyState.TodayClaimed = true;
                }
                else if (dailyState.LastClaimedDate.HasValue && dailyState.LastClaimedDate.Value.Date < today)
                {
                    dailyState.TodayClaimed = false;
                }
            }

            // สร้างปฏิทินรางวัล 7 วัน
            var rewards = new List<DailyRewardItem>
            {
                new(Day: 1, Coins: 50, Claimed: dailyState.CurrentStreak >= 1 && dailyState.TodayClaimed),
                new(Day: 2, Coins: 75, Claimed: dailyState.CurrentStreak >= 2 && dailyState.TodayClaimed),
                new(Day: 3, Coins: 100, Claimed: dailyState.CurrentStreak >= 3 && dailyState.TodayClaimed),
                new(Day: 4, Coins: 125, Claimed: dailyState.CurrentStreak >= 4 && dailyState.TodayClaimed),
                new(Day: 5, Coins: 150, Claimed: dailyState.CurrentStreak >= 5 && dailyState.TodayClaimed),
                new(Day: 6, Coins: 200, Claimed: dailyState.CurrentStreak >= 6 && dailyState.TodayClaimed),
                new(Day: 7, Coins: 300, Claimed: dailyState.CurrentStreak >= 7 && dailyState.TodayClaimed)
            };

            return Ok(new DailyRewardsResponse(
                CurrentStreak: dailyState.CurrentStreak,
                TodayClaimed: dailyState.TodayClaimed,
                Rewards: rewards));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching daily rewards.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while loading daily rewards."
            });
        }
    }

    // ==========================================
    // POST /api/v1/daily-rewards/claim (รับรางวัลประจำวัน)
    // ==========================================
    [HttpPost("daily-rewards/claim")]
    [ProducesResponseType(typeof(ClaimDailyRewardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClaimDailyRewardResponse>> ClaimDailyReward(CancellationToken cancellationToken)
    {
        try
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(subject, out var playerPk)) return UnauthorizedProblem();

            var user = await database.Users.SingleOrDefaultAsync(u => u.Id == playerPk, cancellationToken);
            if (user is null) return UnauthorizedProblem();
            if (user.IsSuspended) return ForbiddenProblem();

            var dailyState = await database.PlayerDailyStates
                .SingleOrDefaultAsync(d => d.PlayerPK == playerPk, cancellationToken);

            var today = DateTimeOffset.UtcNow.Date;

            if (dailyState is null)
            {
                dailyState = new PlayerDailyState
                {
                    PK = Guid.NewGuid(),
                    Code = $"DLY_{Guid.NewGuid():N}".Substring(0, 16),
                    Status = "ACTIVE",
                    PlayerPK = playerPk,
                    CurrentStreak = 0,
                    TodayClaimed = false
                };
                database.PlayerDailyStates.Add(dailyState);
            }

            // ตรวจสอบว่าวันนี้รับไปแล้วหรือยัง
            if (dailyState.LastClaimedDate.HasValue && dailyState.LastClaimedDate.Value.Date == today)
            {
                return Conflict(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = "Daily reward for today has already been claimed."
                });
            }

            // คำนวณ Streak และรางวัลเหรียญ
            if (dailyState.LastClaimedDate.HasValue && dailyState.LastClaimedDate.Value.Date == today.AddDays(-1))
            {
                dailyState.CurrentStreak = (dailyState.CurrentStreak % 7) + 1;
            }
            else
            {
                dailyState.CurrentStreak = 1;
            }

            int rewardCoins = dailyState.CurrentStreak switch
            {
                1 => 50,
                2 => 75,
                3 => 100,
                4 => 125,
                5 => 150,
                6 => 200,
                7 => 300,
                _ => 50
            };

            user.Coins += rewardCoins;
            dailyState.TodayClaimed = true;
            dailyState.LastClaimedDate = DateTimeOffset.UtcNow;
            dailyState.UpdatedDate = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation(GameEvent, "Player {PlayerId} claimed daily reward. Streak: {Streak}, Coins: {Coins}", playerPk, dailyState.CurrentStreak, rewardCoins);

            return Ok(new ClaimDailyRewardResponse(
                RewardCoins: rewardCoins,
                CurrentStreak: dailyState.CurrentStreak,
                TotalCoins: user.Coins));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while claiming daily reward.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while claiming daily reward."
            });
        }
    }

    // DTO Records เพิ่มเติมสำหรับ Daily Rewards
    public sealed record DailyRewardItem(int Day, int Coins, bool Claimed);
    public sealed record DailyRewardsResponse(int CurrentStreak, bool TodayClaimed, List<DailyRewardItem> Rewards);
    public sealed record ClaimDailyRewardResponse(int RewardCoins, int CurrentStreak, int TotalCoins);

     // DTO Records สำหรับ Missions
    public sealed record MissionsResponse(List<MissionItem> Missions);
    public sealed record MissionItem(
        string Id,
        string Title,
        string Description,
        int Progress,
        int Target,
        int RewardCoins,
        bool IsCompleted,
        bool IsClaimed);
    public sealed record ClaimMissionResponse(
        string MissionId,
        int CoinsAwarded,
        int TotalCoins);

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