using System.ComponentModel.DataAnnotations;

namespace SpaceEventHub.API.DTOs;

public class EventDTO
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string City { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AreaOfInterest { get; set; } = string.Empty;
    public string? RegistrationLink { get; set; }
    public int OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public int RegistrationCount { get; set; }
    public bool IsUserRegistered { get; set; }
}

public class CreateEventRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AreaOfInterest { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? RegistrationLink { get; set; }
}

public class UpdateEventRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AreaOfInterest { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? RegistrationLink { get; set; }
}

public class EventSearchRequest
{
    public string? City { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? AreaOfInterest { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
