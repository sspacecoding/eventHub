namespace SpaceEventHub.API.DTOs;

public class NotificationDTO
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public int? EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? EventTitle { get; set; }
}

public class AnalyticsOverviewDTO
{
    public int TotalVisitors { get; set; }
    public int TotalPageViews { get; set; }
    public int TotalEvents { get; set; }
    public int TotalRegistrations { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveEvents { get; set; }
}

public class PageViewStatsDTO
{
    public string Date { get; set; } = string.Empty;
    public int ViewCount { get; set; }
}

public class RegistrationStatsDTO
{
    public string EventTitle { get; set; } = string.Empty;
    public int RegistrationCount { get; set; }
}

public class DatabaseTableDTO
{
    public string TableName { get; set; } = string.Empty;
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}

public class RegistrationDTO
{
    public int RegistrationId { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public EventDTO? Event { get; set; }
}
