using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceEventHub.API.Models;

public class EventRegistration
{
    [Key]
    public int RegistrationId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string Status { get; set; } = "Registered"; // Registered, Cancelled, Attended

    // Navigation properties
    [ForeignKey("EventId")]
    public virtual Event? Event { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
