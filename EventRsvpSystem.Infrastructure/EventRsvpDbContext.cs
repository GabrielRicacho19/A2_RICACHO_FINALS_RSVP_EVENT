using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventRsvpSystem.Infrastructure;

public class EventRsvpDbContext : IdentityDbContext
{
    public EventRsvpDbContext(DbContextOptions<EventRsvpDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Rsvp> Rsvps => Set<Rsvp>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Event>()
            .HasMany(e => e.Rsvps)
            .WithOne(r => r.Event!)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
