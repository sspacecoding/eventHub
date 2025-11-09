using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceEventHub.API.Data;
using SpaceEventHub.API.DTOs;

namespace SpaceEventHub.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly SpaceEventHubContext _context;

    public AdminController(SpaceEventHubContext context)
    {
        _context = context;
    }

    [HttpGet("database-view")]
    public async Task<IActionResult> GetDatabaseView()
    {
        var tables = new List<DatabaseTableDTO>();

        // Users table
        var usersData = await _context.Users.ToListAsync();
        var users = usersData.Select(u => new Dictionary<string, object?>
        {
            ["UserId"] = u.UserId,
            ["Email"] = u.Email,
            ["FirstName"] = u.FirstName,
            ["LastName"] = u.LastName,
            ["Role"] = u.Role,
            ["IsActive"] = u.IsActive,
            ["CreatedAt"] = u.CreatedAt
        }).ToList();

        tables.Add(new DatabaseTableDTO
        {
            TableName = "Users",
            Rows = users
        });

        // Events table
        var eventsData = await _context.Events
            .Include(e => e.Organizer)
            .ToListAsync();
        
        var events = eventsData.Select(e => new Dictionary<string, object?>
        {
            ["EventId"] = e.EventId,
            ["Title"] = e.Title,
            ["City"] = e.City,
            ["AreaOfInterest"] = e.AreaOfInterest,
            ["EventDate"] = e.EventDate,
            ["Organizer"] = e.Organizer?.FullName,
            ["IsActive"] = e.IsActive,
            ["CreatedAt"] = e.CreatedAt
        }).ToList();

        tables.Add(new DatabaseTableDTO
        {
            TableName = "Events",
            Rows = events
        });

        // EventRegistrations table
        var registrationsData = await _context.EventRegistrations
            .Include(r => r.Event)
            .Include(r => r.User)
            .ToListAsync();
        
        var registrations = registrationsData.Select(r => new Dictionary<string, object?>
        {
            ["RegistrationId"] = r.RegistrationId,
            ["Event"] = r.Event?.Title,
            ["User"] = r.User?.FullName,
            ["Status"] = r.Status,
            ["RegisteredAt"] = r.RegisteredAt
        }).ToList();

        tables.Add(new DatabaseTableDTO
        {
            TableName = "EventRegistrations",
            Rows = registrations
        });

        // Notifications table
        var notificationsData = await _context.Notifications
            .Include(n => n.User)
            .Take(50)
            .ToListAsync();
        
        var notifications = notificationsData.Select(n => new Dictionary<string, object?>
        {
            ["NotificationId"] = n.NotificationId,
            ["User"] = n.User?.FullName,
            ["Title"] = n.Title,
            ["IsRead"] = n.IsRead,
            ["CreatedAt"] = n.CreatedAt
        }).ToList();

        tables.Add(new DatabaseTableDTO
        {
            TableName = "Notifications",
            Rows = notifications
        });

        // PageViews table
        var pageViewsData = await _context.PageViews
            .OrderByDescending(pv => pv.ViewedAt)
            .Take(100)
            .ToListAsync();
        
        var pageViews = pageViewsData.Select(pv => new Dictionary<string, object?>
        {
            ["ViewId"] = pv.ViewId,
            ["PageUrl"] = pv.PageUrl,
            ["IpAddress"] = pv.IpAddress,
            ["ViewedAt"] = pv.ViewedAt
        }).ToList();

        tables.Add(new DatabaseTableDTO
        {
            TableName = "PageViews",
            Rows = pageViews
        });

        return Ok(tables);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.UserId,
                u.Email,
                u.FirstName,
                u.LastName,
                u.Role,
                u.IsActive,
                u.CreatedAt,
                u.LastLoginAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Select(e => new
            {
                e.EventId,
                e.Title,
                e.Description,
                e.EventDate,
                e.City,
                e.Location,
                e.AreaOfInterest,
                OrganizerName = e.Organizer!.FullName,
                RegistrationCount = e.Registrations.Count,
                e.IsActive,
                e.CreatedAt
            })
            .ToListAsync();

        return Ok(events);
    }
}
