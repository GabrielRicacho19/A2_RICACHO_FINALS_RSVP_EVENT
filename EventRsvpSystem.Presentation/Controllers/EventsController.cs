using System.Security.Claims;
using EventRsvpSystem.Infrastructure;
using EventRsvpSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventRsvpSystem.Presentation.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetUpcomingEventsAsync(cancellationToken);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = events.Select(e => new EventListItemViewModel
        {
            Id = e.Id,
            Name = e.Name,
            Date = e.Date,
            Capacity = e.Capacity,
            RsvpCount = e.Rsvps.Count,
            IsFull = e.Rsvps.Count >= e.Capacity,
            HasUserRsvped = userId != null && e.Rsvps.Any(r => r.UserId == userId)
        }).ToList();

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetEventAsync(id, cancellationToken);
        if (ev is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = new EventDetailsViewModel
        {
            Id = ev.Id,
            Name = ev.Name,
            Date = ev.Date,
            Capacity = ev.Capacity,
            RsvpCount = ev.Rsvps.Count,
            IsFull = ev.Rsvps.Count >= ev.Capacity,
            HasUserRsvped = userId != null && ev.Rsvps.Any(r => r.UserId == userId)
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new CreateEventViewModel
        {
            Date = DateTime.UtcNow.AddDays(7)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _eventService.CreateEventAsync(model.Name, model.Date, model.Capacity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rsvp(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Challenge();
        }

        var success = await _eventService.RsvpAsync(id, userId, cancellationToken);

        if (!success)
        {
            TempData["RsvpError"] = "Unable to RSVP for this event. It may be full or no longer available.";
        }
        else
        {
            TempData["RsvpSuccess"] = "Your RSVP has been recorded.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}

public class EventListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public int RsvpCount { get; set; }
    public bool IsFull { get; set; }
    public bool HasUserRsvped { get; set; }
}

public class EventDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public int RsvpCount { get; set; }
    public bool IsFull { get; set; }
    public bool HasUserRsvped { get; set; }
}

public class CreateEventViewModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
}
