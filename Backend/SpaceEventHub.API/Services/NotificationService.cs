using Microsoft.EntityFrameworkCore;
using SpaceEventHub.API.Data;
using SpaceEventHub.API.DTOs;
using SpaceEventHub.API.Models;

namespace SpaceEventHub.API.Services;

public interface INotificationService
{
    Task<List<NotificationDTO>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
    Task<NotificationDTO?> MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task CreateNotificationAsync(int userId, int? eventId, string title, string message);
}

public class NotificationService : INotificationService
{
    private readonly SpaceEventHubContext _context;

    public NotificationService(SpaceEventHubContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
    {
        var query = _context.Notifications
            .Include(n => n.Event)
            .Where(n => n.UserId == userId);

        if (unreadOnly)
            query = query.Where(n => !n.IsRead);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        return notifications.Select(n => new NotificationDTO
        {
            NotificationId = n.NotificationId,
            UserId = n.UserId,
            EventId = n.EventId,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            EventTitle = n.Event?.Title
        }).ToList();
    }

    public async Task<NotificationDTO?> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .Include(n => n.Event)
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null)
            return null;

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return new NotificationDTO
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            EventId = notification.EventId,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            EventTitle = notification.Event?.Title
        };
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task CreateNotificationAsync(int userId, int? eventId, string title, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            EventId = eventId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
