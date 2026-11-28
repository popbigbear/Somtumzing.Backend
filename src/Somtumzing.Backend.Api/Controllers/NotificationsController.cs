using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public sealed class NotificationsController(ILogger<NotificationsController> logger) : ControllerBase
{
    // ตัวอย่างจำลอง Store (ในระบบจริงควรดึงจาก Database ตาม UserId)
    private static readonly Dictionary<string, List<AppNotification>> UserNotificationsStore = [];

    [HttpGet]
    [ProducesResponseType(typeof(NotificationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetNotifications(CancellationToken cancellationToken)
    {
        try
        {
            // ดึง UserId จาก sub claim หรือ NameIdentifier
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("Unauthorized access attempt to notifications: UserId/sub claim is missing.");
                return Unauthorized(new { message = "Invalid authentication token." });
            }

            // ตัวอย่างข้อมูลจำลอง (Mock Data) สำหรับผู้ใช้แต่ละราย
            if (!UserNotificationsStore.TryGetValue(userId, out var notifications))
            {
                notifications =
                [
                    new AppNotification
                    {
                        Id = "welcome",
                        Title = "ยินดีต้อนรับสู่ส้มตำซิ่ง!",
                        Description = "เริ่มเปิดร้านรถเข็นของคุณแล้วลุยกันเลย",
                        Kind = "system",
                        Read = false,
                        CreatedAt = DateTime.UtcNow.AddHours(-2)
                    },
                    new AppNotification
                    {
                        Id = "tip",
                        Title = "เคล็ดลับทำกำไร",
                        Description = "อัปเกรดร่มแม่ค้าเพื่อเพิ่มโบนัสรายได้ +10%",
                        Kind = "game",
                        Read = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                ];
                UserNotificationsStore[userId] = notifications;
            }

            var unreadCount = notifications.Count(n => !n.Read);

            var response = new NotificationsResponse
            {
                Notifications = notifications,
                UnreadCount = unreadCount
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching notifications.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred." });
        }
    }

    [HttpPost("{id}/read")]
    [ProducesResponseType(typeof(NotificationActionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult MarkAsRead(string id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Invalid authentication token." });
            }

            if (!UserNotificationsStore.TryGetValue(userId, out var notifications))
            {
                return NotFound(new { message = "Notification not found." });
            }

            var notification = notifications.FirstOrDefault(n => n.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (notification is null)
            {
                return NotFound(new { message = "Notification not found." });
            }

            notification.Read = true;
            return Ok(new NotificationActionResponse { Success = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking notification {NotificationId} as read.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred." });
        }
    }

    [HttpPost("read-all")]
    [ProducesResponseType(typeof(NotificationActionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult MarkAllAsRead(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Invalid authentication token." });
            }

            if (UserNotificationsStore.TryGetValue(userId, out var notifications))
            {
                foreach (var notification in notifications)
                {
                    notification.Read = true;
                }
            }

            return Ok(new NotificationActionResponse { Success = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking all notifications as read.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal error occurred." });
        }
    }
}