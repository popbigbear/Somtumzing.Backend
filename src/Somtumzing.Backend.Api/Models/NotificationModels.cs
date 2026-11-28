namespace Somtumzing.Backend.Api.Models;

public class AppNotification
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty; // "system" | "game" | "reward"
    public bool Read { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationsResponse
{
    public List<AppNotification> Notifications { get; set; } = [];
    public int UnreadCount { get; set; }
}

public class NotificationActionResponse
{
    public bool Success { get; set; }
}