using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceEventHub.API.DTOs;
using SpaceEventHub.API.Services;
using System.Security.Claims;

namespace SpaceEventHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchEvents([FromQuery] EventSearchRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? currentUserId = userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;

        var events = await _eventService.SearchEventsAsync(request, currentUserId);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? currentUserId = userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;

        var eventItem = await _eventService.GetEventByIdAsync(id, currentUserId);

        if (eventItem == null)
            return NotFound();

        return Ok(eventItem);
    }

    [Authorize(Roles = "Organizer,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var eventItem = await _eventService.CreateEventAsync(request, userId);

        if (eventItem == null)
            return BadRequest(new { message = "Failed to create event" });

        return CreatedAtAction(nameof(GetEvent), new { id = eventItem.EventId }, eventItem);
    }

    [Authorize(Roles = "Organizer,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId) || userRole == null)
            return Unauthorized();

        var eventItem = await _eventService.UpdateEventAsync(id, request, userId, userRole);

        if (eventItem == null)
            return NotFound();

        return Ok(eventItem);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId) || userRole == null)
            return Unauthorized();

        var result = await _eventService.DeleteEventAsync(id, userId, userRole);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForEvent(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var registration = await _eventService.RegisterForEventAsync(id, userId);

        if (registration == null)
            return BadRequest(new { message = "Failed to register for event or already registered" });

        return Ok(registration);
    }

    [Authorize]
    [HttpDelete("{id}/register")]
    public async Task<IActionResult> UnregisterFromEvent(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var result = await _eventService.UnregisterFromEventAsync(id, userId);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpGet("my-registrations")]
    public async Task<IActionResult> GetMyRegistrations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var registrations = await _eventService.GetUserRegistrationsAsync(userId);
        return Ok(registrations);
    }

    [Authorize(Roles = "Organizer,Admin")]
    [HttpGet("my-events")]
    public async Task<IActionResult> GetMyEvents()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var events = await _eventService.GetOrganizerEventsAsync(userId);
        return Ok(events);
    }
}
