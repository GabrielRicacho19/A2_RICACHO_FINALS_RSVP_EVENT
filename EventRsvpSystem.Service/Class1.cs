namespace EventRsvpSystem.Service;

using EventRsvpSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface IEventService
{
    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
    Task<Event?> GetEventAsync(int id, CancellationToken cancellationToken = default);
    Task<Event> CreateEventAsync(string name, DateTime date, int capacity, CancellationToken cancellationToken = default);
    Task<bool> RsvpAsync(int eventId, string userId, CancellationToken cancellationToken = default);
}

public class EventService : IEventService
{
    private readonly EventRsvpDbContext _db;

    public EventService(EventRsvpDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _db.Events
            .Include(e => e.Rsvps)
            .Where(e => e.Date >= now)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetEventAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Events
            .Include(e => e.Rsvps)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Event> CreateEventAsync(string name, DateTime date, int capacity, CancellationToken cancellationToken = default)
    {
        var entity = new Event
        {
            Name = name,
            Date = date,
            Capacity = capacity
        };

        _db.Events.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RsvpAsync(int eventId, string userId, CancellationToken cancellationToken = default)
    {
        var ev = await _db.Events
            .Include(e => e.Rsvps)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (ev is null)
        {
            return false;
        }

        if (ev.Rsvps.Count >= ev.Capacity)
        {
            return false; // event is full
        }

        if (ev.Rsvps.Any(r => r.UserId == userId))
        {
            return true; // already RSVPed, treat as success
        }

        var rsvp = new Rsvp
        {
            EventId = eventId,
            UserId = userId
        };

        _db.Rsvps.Add(rsvp);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
