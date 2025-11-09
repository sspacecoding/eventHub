using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceEventHub.API.Models;

public class PageView
{
    [Key]
    public int ViewId { get; set; }

    [Required]
    [MaxLength(500)]
    public string PageUrl { get; set; } = string.Empty;

    public int? UserId { get; set; }

    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
