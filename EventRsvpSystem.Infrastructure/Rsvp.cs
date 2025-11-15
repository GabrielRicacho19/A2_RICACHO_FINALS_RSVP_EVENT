using System.ComponentModel.DataAnnotations;

namespace EventRsvpSystem.Infrastructure;

public class Rsvp
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
