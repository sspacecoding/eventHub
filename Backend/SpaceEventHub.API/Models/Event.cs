using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceEventHub.API.Models;

public class Event
{
    [Key]
    public int EventId { get; set; }

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
    public string AreaOfInterest { get; set; } = string.Empty; // AI, Backend, UX, Frontend, DevOps, etc.

    [MaxLength(500)]
    public string? RegistrationLink { get; set; }

    [Required]
    public int OrganizerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("OrganizerId")]
    public virtual User? Organizer { get; set; }

    public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
