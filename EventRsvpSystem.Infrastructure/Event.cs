using System.ComponentModel.DataAnnotations;

namespace EventRsvpSystem.Infrastructure;

public class Event
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public ICollection<Rsvp> Rsvps { get; set; } = new List<Rsvp>();
}
