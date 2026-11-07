using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class PlayerAuthController(
    GameDbContext database,
    UserManager<ApplicationUser> userManager,
    AccessTokenService accessTokens,
    IAuditLogWriter auditLogWriter,
    ILogger<PlayerAuthController> logger) : ControllerBase
{
    private static readonly EventId PlayerLoginEvent = new(1001, "PlayerLogin");
    private static readonly EventId PlayerTokenRefreshEvent = new(1002, "PlayerTokenRefresh");
    private static readonly EventId PlayerLogoutEvent = new(1003, "PlayerLogout");
    private static readonly EventId PlayerTokenRefreshRejectedEvent = new(1004, "PlayerTokenRefreshRejected");


    // เพิ่ม EventId สำหรับ Player Profile เพื่อแก้ปัญหา error CS0103
    private static readonly EventId PlayerProfileEvent = new(1005, "PlayerProfileEvent");

    [HttpPost("createGuest")]
    [EnableRateLimiting("guest-bootstrap")]
    public async Task<ActionResult<AuthSessionResponse>> CreateGuest(CancellationToken cancellationToken)
    {

        var now = DateTimeOffset.UtcNow;

        var userId = Guid.NewGuid();



        var user = new ApplicationUser

        {

            Id = userId,

            UserName = $"guest-{Guid.NewGuid():N}",

            DisplayName = $"ร้านใหม่ #{Guid.NewGuid():N}".Substring(0, 4).ToUpper(),

            CreatedAtUtc = now,

            SecurityStamp = Guid.NewGuid().ToString("D"),

            ConcurrencyStamp = Guid.NewGuid().ToString("D"),

        };


        // แฮชรหัสผ่านให้เรียบร้อยก่อน Add เข้า DbContext เพื่อให้บันทึกในรอบเดียวจบ (Single Round-trip)

        var password = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48)) + "aA1!";

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        user.PasswordHash = passwordHasher.HashPassword(user, password);


        // เพิ่ม User เข้า DbSet

        database.Users.Add(user);


        // สร้าง Refresh Token รอไว้เลยเพื่อบันทึกพร้อมกันใน Transaction เดียว

        var refresh = RefreshTokenFactory.Create(userId, now);

        database.RefreshTokens.Add(refresh.Entity);


        // บันทึกครั้งเดียวจบ (Atomic Operation) ป้องกันการกระตุ้น Trigger ซ้ำซ้อน

        await database.SaveChangesAsync(cancellationToken);


        // กำหนด Role หลังจากบันทึกตัว User ลงฐานข้อมูลเรียบร้อยแล้ว (UserManager จำเป็นต้องใช้ User ที่มีสถานะ Attached/Persisted)

        var roleResult = await userManager.AddToRoleAsync(user, "Player");

        EnsureSucceeded(roleResult, "assigning the player role");


        await auditLogWriter.WriteAsync(

            "PlayerLogin",

            "Information",

            "GuestCreated",

            cancellationToken,

            actorUserId: user.Id,

            actorUsername: user.UserName,

            targetType: "Player",

            targetId: user.Id.ToString("D"));


        logger.LogInformation(

            PlayerLoginEvent,

            "Player authentication completed. {EventType} {PlayerId} {Outcome}",

            "PlayerLogin",

            user.Id,

            "GuestCreated");


        return Ok(new AuthSessionResponse(

            user.Id,

            accessTokens.CreatePlayerToken(user, now),

            refresh.PlainText,

            now.AddMinutes(15),

            refresh.Entity.ExpiresAtUtc));

    }

    // ==========================================
    // Guest Login Endpoint (Updated with DeviceId tracking & RFC 7807 Error format)
    // ==========================================
    [HttpPost("guest")]
    [EnableRateLimiting("guest-bootstrap")]
    public async Task<ActionResult<GuestAuthResponse>> GuestLogin(
        [FromBody] GuestLoginRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Request parameters are invalid."
            });
        }

        try
        {
            var now = DateTimeOffset.UtcNow;
            var guestUserName = $"guest-{request.DeviceId}";

            // 1. ค้นหาผู้เล่นเดิมจาก DeviceId
            var existingUser = await database.Users
                .SingleOrDefaultAsync(u => u.UserName == guestUserName, cancellationToken);

            ApplicationUser user;
            if (existingUser is not null)
            {
                if (existingUser.IsSuspended)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807",
                        Title = "Forbidden",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "Account is suspended."
                    });
                }
                user = existingUser;
            }
            else
            {
                // 2. สร้างผู้เล่นใหม่โดยกำหนดค่าทั้งหมดครั้งเดียวจบ แล้วสั่ง SaveChangesAsync ครั้งเดียวเพื่อไม่ให้ Trigger ทำงานซ้ำซ้อน
                var defaultDisplayName = $"ร้านใหม่ #{Guid.NewGuid():N}".Substring(0, 4).ToUpper();
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = guestUserName,
                    DisplayName = defaultDisplayName,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    CreatedAtUtc = now
                };

                var password = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48)) + "aA1!";
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, password);

                // บันทึกครั้งเดียวจบ ไม่มีการ UPDATE ซ้ำซ้อน
                database.Users.Add(user);
                await database.SaveChangesAsync(cancellationToken);

                await userManager.AddToRoleAsync(user, "Player");
            }

            var accessToken = accessTokens.CreatePlayerToken(user, now);

            logger.LogInformation(
                PlayerLoginEvent,
                "Guest login completed for device {DeviceId} on platform {Platform}",
                request.DeviceId,
                request.Platform);

            return Ok(new GuestAuthResponse(
                Token: $"mock_p_{accessToken.Substring(0, Math.Min(16, accessToken.Length))}",
                Player: new PlayerResponse(
                    Id: $"p_{user.Id:N}".Substring(0, 12),
                    Username: user.UserName,
                    DisplayName: user.DisplayName ?? user.UserName ?? "ผู้เล่นใหม่",
                    Coins: 100,
                    BestScore: 0,
                    TotalServed: 0,
                    Upgrades: new Dictionary<string, int>(),
                    CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                )));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during guest login.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Backend Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            });
        }
    }




    [HttpPost("refresh")]
    public async Task<ActionResult<AuthSessionResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Refresh token must be between 64 and 128 characters."
            });
        }

        var now = DateTimeOffset.UtcNow;
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);

        var tokenHash = RefreshTokenFactory.Hash(request.RefreshToken);
        var token = await database.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .SingleOrDefaultAsync(
                refreshToken => refreshToken.TokenHash == tokenHash,
                cancellationToken);

        if (token is null || token.RevokedAtUtc is not null || token.ExpiresAtUtc <= now)
        {
            await auditLogWriter.WriteAsync(
                "PlayerTokenRefreshRejected",
                "Warning",
                "InvalidOrExpired",
                cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogWarning(
                PlayerTokenRefreshRejectedEvent,
                "Player refresh was rejected. {EventType} {Outcome}",
                "PlayerTokenRefreshRejected",
                "InvalidOrExpired");

            // ส่งคืน Error 401 ตามมาตรฐาน RFC 7807
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Refresh token is invalid, revoked, or expired. Please login again."
            });
        }

        if (token.User.IsSuspended)
        {
            await auditLogWriter.WriteAsync(
                "PlayerTokenRefreshRejected",
                "Warning",
                "AccountSuspended",
                cancellationToken,
                actorUserId: token.UserId,
                actorUsername: token.User.UserName,
                targetType: "Player",
                targetId: token.UserId.ToString("D"));
            await transaction.CommitAsync(cancellationToken);

            logger.LogWarning(
                PlayerTokenRefreshRejectedEvent,
                "Player refresh was rejected. {EventType} {PlayerId} {Outcome}",
                "PlayerTokenRefreshRejected",
                token.UserId,
                "AccountSuspended");

            // ส่งคืน Error 403 ตามมาตรฐาน RFC 7807
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Account is suspended."
            });
        }

        token.RevokedAtUtc = now;
        token.Status = "REVOKED";
        var next = RefreshTokenFactory.Create(token.UserId, now);
        token.ReplacedByTokenId = next.Entity.Id;
        database.RefreshTokens.Add(next.Entity);

        try
        {
            await auditLogWriter.WriteAsync(
                "PlayerTokenRefresh",
                "Information",
                "Succeeded",
                cancellationToken,
                actorUserId: token.UserId,
                actorUsername: token.User.UserName,
                targetType: "Player",
                targetId: token.UserId.ToString("D"));
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogWarning(
                PlayerTokenRefreshRejectedEvent,
                "Concurrent player refresh was rejected. {EventType} {Outcome}",
                "PlayerTokenRefreshRejected",
                "ConcurrentUse");

            return Conflict(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = "Concurrent refresh token usage detected."
            });
        }

        logger.LogInformation(
            PlayerTokenRefreshEvent,
            "Player refresh completed. {EventType} {PlayerId} {Outcome}",
            "PlayerTokenRefresh",
            token.UserId,
            "Succeeded");

        return Ok(new AuthSessionResponse(
            token.UserId,
            accessTokens.CreatePlayerToken(token.User, now),
            next.PlainText,
            now.AddMinutes(15),
            next.Entity.ExpiresAtUtc));
    }

    [Authorize(Roles = "Player")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Refresh token is required and must be valid."
            });
        }

        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Token is expired or invalid."
            });
        }

        var hash = RefreshTokenFactory.Hash(request.RefreshToken);
        var token = await database.RefreshTokens.SingleOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == hash && refreshToken.UserId == userId,
            cancellationToken);

        if (token is null || token.RevokedAtUtc is not null)
        {
            // หากไม่พบโทเค็นหรือถูกเพิกถอนไปแล้ว ให้ตอบกลับ 200 OK ตามหลัก Idempotency เพื่อความปลอดภัย
            return Ok();
        }

        token.RevokedAtUtc = DateTimeOffset.UtcNow;
        token.Status = "REVOKED";

        try
        {
            await database.SaveChangesAsync(cancellationToken);
            await auditLogWriter.WriteAsync(
                "PlayerLogout",
                "Information",
                "Succeeded",
                cancellationToken,
                actorUserId: userId,
                actorUsername: User.Identity?.Name,
                targetType: "Player",
                targetId: userId.ToString("D"));
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning(
                PlayerLogoutEvent,
                "Concurrent player logout was ignored. {EventType} {PlayerId} {Outcome}",
                "PlayerLogout",
                userId,
                "AlreadyRevoked");
            return Ok();
        }

        logger.LogInformation(
            PlayerLogoutEvent,
            "Player logged out. {EventType} {PlayerId} {Outcome}",
            "PlayerLogout",
            userId,
            "Succeeded");

        return Ok();
    }

    [Authorize(Roles = "Player")]
    [HttpPatch("me")]
    public async Task<ActionResult<PlayerResponse>> UpdateDisplayName(
     [FromBody] UpdateDisplayNameRequest request,
     CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Display name must be between 2 and 24 characters."
            });
        }

        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Token is expired or invalid."
            });
        }

        var trimmedName = request.DisplayName.Trim();
        var user = await database.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Player not found."
            });
        }

        if (user.IsSuspended)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Account is suspended."
            });
        }

        // อัปเดตเฉพาะ displayName โดยไม่กระทบ username หลัก
        user.DisplayName = trimmedName;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Failed to update display name."
            });
        }

        return Ok(new PlayerResponse(
            Id: $"p_{user.Id:N}".Substring(0, 12),
            Username: user.UserName,
            DisplayName: user.DisplayName ?? user.UserName,
            Coins: 100,
            BestScore: 0,
            TotalServed: 0,
            Upgrades: new Dictionary<string, int>(),
            CreatedAt: user.CreatedAtUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        ));
    }

    // เพิ่ม Helper Methods สำหรับจัดการ Error 401 และ 403 ภายในคลาสนี้
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



    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Code));
            throw new InvalidOperationException($"Identity failed while {operation}: {errors}");
        }
    }

    public sealed record AuthSessionResponse(
       Guid PlayerId,
       string AccessToken,
       string RefreshToken,
       DateTimeOffset AccessTokenExpiresAtUtc,
       DateTimeOffset RefreshTokenExpiresAtUtc);

    public sealed record PlayerProfileResponse(
        Guid Id,
        string? Username,
        DateTimeOffset CreatedAtUtc);
    public sealed record GuestLoginRequest(
        [Required]
        [StringLength(128, MinimumLength = 8)]
        string DeviceId,
        [Required]
        [RegularExpression("^(web|ios|android)$", ErrorMessage = "Platform must be 'web', 'ios', or 'android'.")]
        string Platform);

    public sealed record GuestAuthResponse(
       string Token,
       PlayerResponse Player);

    public sealed record PlayerResponse(
        string Id,
        string? Username,
        string DisplayName,
        int Coins,
        int BestScore,
        int TotalServed,
        Dictionary<string, int> Upgrades,
        string CreatedAt);

    public sealed record UpdateDisplayNameRequest(
    [Required]
    [StringLength(24, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 24 characters.")]
    string DisplayName);

    public sealed class RefreshRequest
    {
        [Required]
        [StringLength(128, MinimumLength = 64)]
        public string RefreshToken { get; init; } = string.Empty;
    }

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