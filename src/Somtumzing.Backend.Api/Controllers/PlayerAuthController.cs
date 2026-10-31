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

    [HttpPost("guest")]
    [EnableRateLimiting("guest-bootstrap")]
    public async Task<ActionResult<AuthSessionResponse>> CreateGuest(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = $"guest-{Guid.NewGuid():N}",
            CreatedAtUtc = now
        };
        var password = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48)) + "aA1!";
        var createResult = await userManager.CreateAsync(user, password);
        EnsureSucceeded(createResult, "creating a guest player");

        var roleResult = await userManager.AddToRoleAsync(user, "Player");
        EnsureSucceeded(roleResult, "assigning the player role");

        var refresh = RefreshTokenFactory.Create(user.Id, now);
        database.RefreshTokens.Add(refresh.Entity);
        await auditLogWriter.WriteAsync(
            "PlayerLogin",
            "Information",
            "GuestCreated",
            cancellationToken,
            actorUserId: user.Id,
            actorUsername: user.UserName,
            targetType: "Player",
            targetId: user.Id.ToString("D"));
        await transaction.CommitAsync(cancellationToken);
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

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthSessionResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var token = await database.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .SingleOrDefaultAsync(
                refreshToken => refreshToken.TokenHash == RefreshTokenFactory.Hash(request.RefreshToken),
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
            return Unauthorized();
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
            return Unauthorized();
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
            return Unauthorized();
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
        if (!Guid.TryParse(User.FindFirstValue("sub"), out var userId))
        {
            return Unauthorized();
        }

        var hash = RefreshTokenFactory.Hash(request.RefreshToken);
        var token = await database.RefreshTokens.SingleOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == hash && refreshToken.UserId == userId,
            cancellationToken);
        if (token is null || token.RevokedAtUtc is not null)
        {
            return NoContent();
        }

        token.RevokedAtUtc = DateTimeOffset.UtcNow;
        token.Status = "REVOKED";
        try
        {
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
            return NoContent();
        }

        logger.LogInformation(
            PlayerLogoutEvent,
            "Player logged out. {EventType} {PlayerId} {Outcome}",
            "PlayerLogout",
            userId,
            "Succeeded");
        return NoContent();
    }

    [Authorize(Roles = "Player")]
    [HttpGet("me")]
    public async Task<ActionResult<PlayerProfileResponse>> GetProfile(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue("sub");
        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await database.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.Id == userId, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new PlayerProfileResponse(user.Id, user.CreatedAtUtc));
    }

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

    public sealed record PlayerProfileResponse(Guid PlayerId, DateTimeOffset CreatedAtUtc);

    public sealed class RefreshRequest
    {
        [Required]
        [StringLength(128, MinimumLength = 64)]
        public string RefreshToken { get; init; } = string.Empty;
    }
}
