using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventRsvpSystem.Infrastructure;
using EventRsvpSystem.Service;
using EventRsvpSystem.Presentation.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use EF Core in-memory database for simplicity in this sample.
builder.Services.AddDbContext<EventRsvpDbContext>(options =>
    options.UseInMemoryDatabase("EventRsvp"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // Basic password requirements; custom validator will enforce stronger rules.
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddPasswordValidator<CustomPasswordValidator<IdentityUser>>()
    .AddEntityFrameworkStores<EventRsvpDbContext>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed a few sample events so the list is not empty on first run.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventRsvpDbContext>();
    if (!db.Events.Any())
    {
        db.Events.AddRange(
            new Event
            {
                Name = "Intro to ASP.NET Core",
                Date = DateTime.UtcNow.AddDays(3),
                Capacity = 50
            },
            new Event
            {
                Name = "Entity Framework Core Deep Dive",
                Date = DateTime.UtcNow.AddDays(7),
                Capacity = 30
            },
            new Event
            {
                Name = "Building Modern Web Apps",
                Date = DateTime.UtcNow.AddDays(14),
                Capacity = 100
            });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
