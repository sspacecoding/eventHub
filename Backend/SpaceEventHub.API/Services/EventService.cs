using Microsoft.EntityFrameworkCore;
using SpaceEventHub.API.Data;
using SpaceEventHub.API.DTOs;
using SpaceEventHub.API.Models;

namespace SpaceEventHub.API.Services;

public interface IEventService
{
    Task<List<EventDTO>> SearchEventsAsync(EventSearchRequest request, int? currentUserId = null);
    Task<EventDTO?> GetEventByIdAsync(int eventId, int? currentUserId = null);
    Task<EventDTO?> CreateEventAsync(CreateEventRequest request, int organizerId);
    Task<EventDTO?> UpdateEventAsync(int eventId, UpdateEventRequest request, int userId, string userRole);
    Task<bool> DeleteEventAsync(int eventId, int userId, string userRole);
    Task<RegistrationDTO?> RegisterForEventAsync(int eventId, int userId);
    Task<bool> UnregisterFromEventAsync(int eventId, int userId);
    Task<List<RegistrationDTO>> GetUserRegistrationsAsync(int userId);
    Task<List<EventDTO>> GetOrganizerEventsAsync(int organizerId);
}

public class EventService : IEventService
{
    private readonly SpaceEventHubContext _context;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public EventService(SpaceEventHubContext context, INotificationService notificationService, IEmailService emailService)
    {
        _context = context;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<List<EventDTO>> SearchEventsAsync(EventSearchRequest request, int? currentUserId = null)
    {
        var query = _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Where(e => e.IsActive);

        // Apply filters
        if (!string.IsNullOrEmpty(request.City))
            query = query.Where(e => e.City.Contains(request.City));

        if (!string.IsNullOrEmpty(request.AreaOfInterest))
            query = query.Where(e => e.AreaOfInterest.Contains(request.AreaOfInterest));

        if (request.FromDate.HasValue)
            query = query.Where(e => e.EventDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(e => e.EventDate <= request.ToDate.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(e => e.Title.Contains(request.SearchTerm) || e.Description.Contains(request.SearchTerm));

        // Order by event date
        query = query.OrderBy(e => e.EventDate);

        // Pagination
        var events = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return events.Select(e => MapToEventDTO(e, currentUserId)).ToList();
    }

    public async Task<EventDTO?> GetEventByIdAsync(int eventId, int? currentUserId = null)
    {
        var eventItem = await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive);

        return eventItem != null ? MapToEventDTO(eventItem, currentUserId) : null;
    }

    public async Task<EventDTO?> CreateEventAsync(CreateEventRequest request, int organizerId)
    {
        var organizer = await _context.Users.FindAsync(organizerId);
        if (organizer == null)
            return null;

        var eventItem = new Event
        {
            Title = request.Title,
            Description = request.Description,
            EventDate = request.EventDate,
            City = request.City,
            Location = request.Location,
            AreaOfInterest = request.AreaOfInterest,
            RegistrationLink = request.RegistrationLink,
            OrganizerId = organizerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        // Create notification for organizer
        await _notificationService.CreateNotificationAsync(
            organizerId,
            eventItem.EventId,
            "Event Published",
            $"Your event \"{eventItem.Title}\" has been successfully published!"
        );

        // Send email notification
        await _emailService.SendEventCreatedEmailAsync(
            organizer.Email,
            organizer.FullName,
            eventItem.Title
        );

        return MapToEventDTO(eventItem, organizerId);
    }

    public async Task<EventDTO?> UpdateEventAsync(int eventId, UpdateEventRequest request, int userId, string userRole)
    {
        var eventItem = await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventItem == null)
            return null;

        // Check permissions
        if (userRole != "Admin" && eventItem.OrganizerId != userId)
            return null;

        eventItem.Title = request.Title;
        eventItem.Description = request.Description;
        eventItem.EventDate = request.EventDate;
        eventItem.City = request.City;
        eventItem.Location = request.Location;
        eventItem.AreaOfInterest = request.AreaOfInterest;
        eventItem.RegistrationLink = request.RegistrationLink;
        eventItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToEventDTO(eventItem, userId);
    }

    public async Task<bool> DeleteEventAsync(int eventId, int userId, string userRole)
    {
        var eventItem = await _context.Events.FindAsync(eventId);

        if (eventItem == null)
            return false;

        // Only admin can delete events
        if (userRole != "Admin")
            return false;

        eventItem.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<RegistrationDTO?> RegisterForEventAsync(int eventId, int userId)
    {
        var eventItem = await _context.Events
            .Include(e => e.Organizer)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive);

        if (eventItem == null)
            return null;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        // Check if already registered
        var existingRegistration = await _context.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (existingRegistration != null)
            return null;

        var registration = new EventRegistration
        {
            EventId = eventId,
            UserId = userId,
            RegisteredAt = DateTime.UtcNow,
            Status = "Registered"
        };

        _context.EventRegistrations.Add(registration);
        await _context.SaveChangesAsync();

        // Create notification for user
        await _notificationService.CreateNotificationAsync(
            userId,
            eventId,
            "Registration Confirmed",
            $"You have successfully registered for \"{eventItem.Title}\""
        );

        // Send email confirmation
        await _emailService.SendRegistrationConfirmationEmailAsync(
            user.Email,
            user.FullName,
            eventItem.Title,
            eventItem.EventDate
        );

        return new RegistrationDTO
        {
            RegistrationId = registration.RegistrationId,
            EventId = registration.EventId,
            UserId = registration.UserId,
            RegisteredAt = registration.RegisteredAt,
            Status = registration.Status,
            Event = MapToEventDTO(eventItem, userId)
        };
    }

    public async Task<bool> UnregisterFromEventAsync(int eventId, int userId)
    {
        var registration = await _context.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (registration == null)
            return false;

        _context.EventRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<RegistrationDTO>> GetUserRegistrationsAsync(int userId)
    {
        var registrations = await _context.EventRegistrations
            .Include(r => r.Event)
            .ThenInclude(e => e!.Organizer)
            .Include(r => r.Event)
            .ThenInclude(e => e!.Registrations)
            .Where(r => r.UserId == userId && r.Event!.IsActive)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync();

        return registrations.Select(r => new RegistrationDTO
        {
            RegistrationId = r.RegistrationId,
            EventId = r.EventId,
            UserId = r.UserId,
            RegisteredAt = r.RegisteredAt,
            Status = r.Status,
            Event = r.Event != null ? MapToEventDTO(r.Event, userId) : null
        }).ToList();
    }

    public async Task<List<EventDTO>> GetOrganizerEventsAsync(int organizerId)
    {
        var events = await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Where(e => e.OrganizerId == organizerId && e.IsActive)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return events.Select(e => MapToEventDTO(e, organizerId)).ToList();
    }

    private EventDTO MapToEventDTO(Event eventItem, int? currentUserId = null)
    {
        var isUserRegistered = currentUserId.HasValue &&
            eventItem.Registrations.Any(r => r.UserId == currentUserId.Value);

        return new EventDTO
        {
            EventId = eventItem.EventId,
            Title = eventItem.Title,
            Description = eventItem.Description,
            EventDate = eventItem.EventDate,
            City = eventItem.City,
            Location = eventItem.Location,
            AreaOfInterest = eventItem.AreaOfInterest,
            RegistrationLink = eventItem.RegistrationLink,
            OrganizerId = eventItem.OrganizerId,
            OrganizerName = eventItem.Organizer?.FullName ?? "",
            CreatedAt = eventItem.CreatedAt,
            UpdatedAt = eventItem.UpdatedAt,
            IsActive = eventItem.IsActive,
            RegistrationCount = eventItem.Registrations.Count,
            IsUserRegistered = isUserRegistered
        };
    }
}
