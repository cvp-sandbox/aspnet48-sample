# Home Page Migration Plan: .NET Framework 4.8 to .NET 9.0

This document outlines the step-by-step process for migrating the home page (HomeController's Index action and associated views) from the legacy .NET Framework 4.8 ASP.NET MVC application to the new .NET 9.0 ASP.NET MVC application.

## Overview

The home page in the legacy application consists of:
- A HomeController with an Index action that fetches featured events
- An Index.cshtml view with multiple sections:
  - Hero/jumbotron section
  - Stats section (partial view)
  - Featured events section (partial view)
  - User-specific upcoming events (partial view, shown when authenticated)

## Migration Steps

### 1. Create Required Models

The legacy home page relies on `Event` and `Registration` models. We need to create these in the .NET 9.0 application:

```csharp
// Models/EventModels.cs
namespace EventRegistrationSystemCore.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property (not stored in DB)
        public int RegistrationCount { get; set; }
    }

    public class Registration
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation property (not stored in DB)
        public string UserName { get; set; }
        public Event Event { get; set; }
    }
}
```

### 2. Create Repository Interfaces and Implementations

Create the repository interfaces and implementations needed for the home page:

```csharp
// Repositories/RepositoryInterfaces.cs
namespace EventRegistrationSystemCore.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetUpcomingEvents(int count);
        IEnumerable<Event> GetFeaturedEvents(int count);
    }

    public interface IRegistrationRepository
    {
        IEnumerable<Registration> GetUserUpcomingEvents(string userId);
    }
}

// Repositories/EventRepository.cs
using Dapper;
using EventRegistrationSystemCore.Models;
using System.Data;

namespace EventRegistrationSystemCore.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Event> GetFeaturedEvents(int count)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate
                    LIMIT @Count", new { Count = count });
            }
            finally
            {
                _connection.Close();
            }
        }

        public IEnumerable<Event> GetUpcomingEvents(int count)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    WHERE e.EventDate > CURRENT_TIMESTAMP
                    ORDER BY e.EventDate
                    LIMIT @Count", new { Count = count });
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}

// Repositories/RegistrationRepository.cs
using Dapper;
using EventRegistrationSystemCore.Models;
using System.Data;

namespace EventRegistrationSystemCore.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly IDbConnection _connection;

        public RegistrationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Registration> GetUserUpcomingEvents(string userId)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Registration, Event, Registration>(@"
                    SELECT r.*, e.*
                    FROM Registrations r
                    JOIN Events e ON r.EventId = e.EventId
                    JOIN AspNetUsers u on u.Id = r.UserId
                    WHERE u.Id = @UserId AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
                    ORDER BY e.EventDate DESC",
                    (registration, eventItem) => 
                    {
                        registration.Event = eventItem;
                        return registration;
                    },
                    new { UserId = userId },
                    splitOn: "EventId");
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
```

### 3. Register Services in Program.cs

Add the repository services to the dependency injection container:

```csharp
// In Program.cs, add these lines after the existing services.AddScoped<> calls:
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
```

### 4. Update HomeController

Update the HomeController in the .NET 9.0 application:

```csharp
// Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventRegistrationSystemCore.Models;
using EventRegistrationSystemCore.Repositories;
using System.Data;
using Dapper;
using System.Security.Claims;

namespace EventRegistrationSystemCore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventRepository _eventRepository;
    private readonly IDbConnection _connection;

    public HomeController(
        ILogger<HomeController> logger,
        IEventRepository eventRepository,
        IDbConnection connection)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _connection = connection;
    }

    public IActionResult Index()
    {
        // Get featured events using the provided SQL query
        IEnumerable<Event> featuredEvents;
        
        _connection.Open();
        try
        {
            featuredEvents = _connection.Query<Event>(@"
                SELECT e.*, 
                (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                FROM Events e
                ORDER BY e.EventDate
                LIMIT 3");
        }
        finally
        {
            _connection.Close();
        }
        
        ViewBag.FeaturedEvents = featuredEvents;
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Stats()
    {
        // Get stats data using the provided SQL query
        dynamic stats;
        
        _connection.Open();
        try
        {
            stats = _connection.Query(@"
                select 'ActiveEvents' as StatLabel, count(*) AS StatValue
                from Events
                where EventDate > CURRENT_TIMESTAMP

                UNION

                select 'ThisWeeksEvents' as StatLabel, count(*) as StatValue
                from Events
                WHERE EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')

                UNION

                select 'RegisteredUsers' as StatLabel, Count(RegistrationId) as StatValue
                from Registrations r 
                join Events e on r.EventId = e.EventId
                where EventDate > CURRENT_TIMESTAMP");
        }
        finally
        {
            _connection.Close();
        }
        
        return PartialView("_Stats", stats);
    }

    public IActionResult FeaturedEvents()
    {
        // Get featured events using the provided SQL query
        IEnumerable<Event> featuredEvents;
        
        _connection.Open();
        try
        {
            featuredEvents = _connection.Query<Event>(@"
                SELECT e.*, 
                (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                FROM Events e
                ORDER BY e.EventDate
                LIMIT 3");
        }
        finally
        {
            _connection.Close();
        }
        
        return PartialView("_FeaturedEvents", featuredEvents);
    }

    public IActionResult UpcomingEvents()
    {
        // If user is not authenticated, return empty result
        if (!User.Identity.IsAuthenticated)
        {
            return PartialView("_UpcomingEvents", new List<Registration>());
        }

        // Get user's upcoming events using the provided SQL query
        IEnumerable<Registration> upcomingEvents;
        
        _connection.Open();
        try
        {
            upcomingEvents = _connection.Query<Registration, Event, Registration>(@"
                SELECT r.*, e.*
                FROM Registrations r
                JOIN Events e ON r.EventId = e.EventId
                JOIN AspNetUsers u on u.Id = r.UserId
                WHERE u.Id = @UserId AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
                ORDER BY e.EventDate DESC",
                (registration, eventItem) => 
                {
                    registration.Event = eventItem;
                    return registration;
                },
                new { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) },
                splitOn: "EventId");
        }
        finally
        {
            _connection.Close();
        }
        
        return PartialView("_UpcomingEvents", upcomingEvents);
    }
}
```

### 5. Create/Update Views

#### 5.1. Update Index.cshtml

```html
@{
    ViewData["Title"] = "Home Page";
}

<main>
    <!-- Hero Section -->
    <div class="jumbotron">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md">
                    <h1 class="display-4">Find events or create your own</h1>
                    <p class="lead">
                        An easy-to-use platform to discover local events and manage registrations.
                        Perfect for both event organizers and attendees.
                    </p>
                    <div class="mt-4">
                        <a asp-controller="Event" asp-action="Index" class="btn btn-primary btn-lg me-2">Browse Events</a>
                        <a asp-controller="Event" asp-action="Create" class="btn btn-success btn-lg me-2">Create Event</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Stats Section -->
    <partial name="_Stats" model="await Component.InvokeAsync("Stats")" />

    <!-- Featured Events Section -->
    <partial name="~/Views/Home/_FeaturedEvents.cshtml" model="await Component.InvokeAsync("FeaturedEvents")" />

    <!-- Upcoming Events - User-specific content (show when logged in) -->
    @if (User.Identity.IsAuthenticated)
    {
        <partial name="~/Views/Home/_UpcomingEvents.cshtml" model="await Component.InvokeAsync("UpcomingEvents")" />
    }
</main>
```

#### 5.2. Create _Stats.cshtml Partial View

```html
@model dynamic

<div class="container mb-5">
    <div class="stats-container">
        <div class="row">
            @foreach (var stat in Model)
            {
                <div class="col-md-4 stat-item">
                    <div class="stat-number">@stat.StatValue</div>
                    @if (stat.StatLabel == "ActiveEvents")
                    {
                        <div>Active Events</div>
                    }
                    else if (stat.StatLabel == "RegisteredUsers")
                    {
                        <div>Registered Users</div>
                    }
                    else if (stat.StatLabel == "ThisWeeksEvents")
                    {
                        <div>Events This Week</div>
                    }
                </div>
            }
        </div>
    </div>
</div>
```

#### 5.3. Create _FeaturedEvents.cshtml Partial View

```html
@model IEnumerable<EventRegistrationSystemCore.Models.Event>

<div class="container mb-5">
    <h2 class="mb-4">Upcoming Events</h2>
    <div class="row row-cols-1 row-cols-md-3 g-4">
        @foreach (var eventItem in Model)
        {
            <div class="col">
                <div class="card h-100 event-card">
                    <div class="card-body">
                        <h5 class="card-title">@eventItem.Name</h5>
                        <p class="card-text">@eventItem.Description</p>
                    </div>
                    <div class="card-footer bg-transparent">
                        <div class="d-flex justify-content-between align-items-center">
                            <small class="text-muted">@eventItem.EventDate.ToString("MMMM d, yyyy")</small>
                            <small class="text-muted">@eventItem.RegistrationCount/@eventItem.MaxAttendees spots</small>
                        </div>
                        <a asp-controller="Event" asp-action="Details" asp-route-id="@eventItem.EventId" class="btn btn-outline-primary mt-2 w-100">View Details</a>
                    </div>
                </div>
            </div>
        }
    </div>
    <div class="text-center mt-4">
        <a asp-controller="Event" asp-action="Index" class="btn btn-outline-primary">View All Events</a>
    </div>
</div>
```

#### 5.4. Create _UpcomingEvents.cshtml Partial View

```html
@model IEnumerable<EventRegistrationSystemCore.Models.Registration>

<div class="container mb-5">
    <div class="card">
        <div class="card-header bg-light">
            <h4 class="mb-0">Your Upcoming Events</h4>
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th>Event</th>
                            <th>Date</th>
                            <th>Location</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        @if (Model != null && Model.Any())
                        {
                            foreach (var registration in Model)
                            {
                                <tr>
                                    <td>@registration.Event.Name</td>
                                    <td>@registration.Event.EventDate.ToString("MMMM d, yyyy")</td>
                                    <td>@registration.Event.Location</td>
                                    <td>
                                        <a asp-controller="Event" asp-action="Details" asp-route-id="@registration.EventId" class="btn btn-sm btn-outline-primary">View</a>
                                        <a asp-controller="Event" asp-action="CancelRegistration" asp-route-id="@registration.RegistrationId" class="btn btn-sm btn-outline-danger">Cancel</a>
                                    </td>
                                </tr>
                            }
                        }
                        else
                        {
                            <tr>
                                <td colspan="4" class="text-center">You have no upcoming events in the next 7 days.</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
```

### 6. Create View Components (Alternative to Html.Action)

Since ASP.NET Core doesn't support `Html.Action`, we'll use View Components instead:

```csharp
// ViewComponents/StatsViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace EventRegistrationSystemCore.ViewComponents
{
    public class StatsViewComponent : ViewComponent
    {
        private readonly IDbConnection _connection;

        public StatsViewComponent(IDbConnection connection)
        {
            _connection = connection;
        }

        public IViewComponentResult Invoke()
        {
            _connection.Open();
            try
            {
                var stats = _connection.Query(@"
                    select 'ActiveEvents' as StatLabel, count(*) AS StatValue
                    from Events
                    where EventDate > CURRENT_TIMESTAMP

                    UNION

                    select 'ThisWeeksEvents' as StatLabel, count(*) as StatValue
                    from Events
                    WHERE EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')

                    UNION

                    select 'RegisteredUsers' as StatLabel, Count(RegistrationId) as StatValue
                    from Registrations r 
                    join Events e on r.EventId = e.EventId
                    where EventDate > CURRENT_TIMESTAMP");
                
                return View("_Stats", stats);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}

// ViewComponents/FeaturedEventsViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using EventRegistrationSystemCore.Models;

namespace EventRegistrationSystemCore.ViewComponents
{
    public class FeaturedEventsViewComponent : ViewComponent
    {
        private readonly IDbConnection _connection;

        public FeaturedEventsViewComponent(IDbConnection connection)
        {
            _connection = connection;
        }

        public IViewComponentResult Invoke()
        {
            _connection.Open();
            try
            {
                var featuredEvents = _connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate
                    LIMIT 3");
                
                return View("_FeaturedEvents", featuredEvents);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}

// ViewComponents/UpcomingEventsViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using EventRegistrationSystemCore.Models;
using System.Security.Claims;

namespace EventRegistrationSystemCore.ViewComponents
{
    public class UpcomingEventsViewComponent : ViewComponent
    {
        private readonly IDbConnection _connection;

        public UpcomingEventsViewComponent(IDbConnection connection)
        {
            _connection = connection;
        }

        public IViewComponentResult Invoke()
        {
            // If user is not authenticated, return empty result
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View("_UpcomingEvents", new List<Registration>());
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            _connection.Open();
            try
            {
                var upcomingEvents = _connection.Query<Registration, Event, Registration>(@"
                    SELECT r.*, e.*
                    FROM Registrations r
                    JOIN Events e ON r.EventId = e.EventId
                    JOIN AspNetUsers u on u.Id = r.UserId
                    WHERE u.Id = @UserId AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
                    ORDER BY e.EventDate DESC",
                    (registration, eventItem) => 
                    {
                        registration.Event = eventItem;
                        return registration;
                    },
                    new { UserId = userId },
                    splitOn: "EventId");
                
                return View("_UpcomingEvents", upcomingEvents);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
```

### 7. Update Index.cshtml to Use View Components

```html
@{
    ViewData["Title"] = "Home Page";
}

<main>
    <!-- Hero Section -->
    <div class="jumbotron">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-md">
                    <h1 class="display-4">Find events or create your own</h1>
                    <p class="lead">
                        An easy-to-use platform to discover local events and manage registrations.
                        Perfect for both event organizers and attendees.
                    </p>
                    <div class="mt-4">
                        <a asp-controller="Event" asp-action="Index" class="btn btn-primary btn-lg me-2">Browse Events</a>
                        <a asp-controller="Event" asp-action="Create" class="btn btn-success btn-lg me-2">Create Event</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Stats Section -->
    @await Component.InvokeAsync("Stats")

    <!-- Featured Events Section -->
    @await Component.InvokeAsync("FeaturedEvents")

    <!-- Upcoming Events - User-specific content (show when logged in) -->
    @if (User.Identity.IsAuthenticated)
    {
        @await Component.InvokeAsync("UpcomingEvents")
    }
</main>
```

### 8. Update Route Configuration

Ensure the default route is properly configured in Program.cs:

```csharp
// Add this after the login route in Program.cs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 9. Update YARP Reverse Proxy Configuration

The YARP Reverse Proxy needs to be updated to route requests for the home page to the new .NET 9.0 application instead of proxying them to the legacy application.

#### 9.1. Update appsettings.json

Modify the `appsettings.json` file in the .NET 9.0 application to update the YARP routes:

```json
{
  "ReverseProxy": {
    "Routes": {
      "homeRoute": {
        "ClusterId": "localCluster",
        "Match": {
          "Path": "/Home/{**catch-all}",
          "Methods": [ "GET", "POST" ]
        },
        "Order": 1
      },
      "rootRoute": {
        "ClusterId": "localCluster",
        "Match": {
          "Path": "/",
          "Methods": [ "GET" ]
        },
        "Order": 2
      },
      "legacyRoute": {
        "ClusterId": "legacyCluster",
        "Match": {
          "Path": "/{**catch-all}",
          "Methods": [ "GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS", "TRACE" ]
        },
        "Order": 999,
        "Transforms": [
          { "PathPattern": "{**catch-all}" }
        ]
      }
    },
    "Clusters": {
      "localCluster": {
        "Destinations": {
          "local": {
            "Address": "https://localhost:{port}/"
          }
        }
      },
      "legacyCluster": {
        "Destinations": {
          "destination1": {
            "Address": "https://localhost:44372/"
          }
        },
        "HttpClient": {
          "DangerousAcceptAnyServerCertificate": true
        }
      }
    }
  }
}
```

Replace `{port}` with the port your .NET 9.0 application is running on.

#### 9.2. Update Program.cs

Uncomment and update the default route in Program.cs to handle the home page requests:

```csharp
// Uncomment and add this before the MapReverseProxy call
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

## Testing the Migration

1. Ensure both applications can run simultaneously
2. Test the home page in the .NET 9.0 application:
   - Verify the layout and styling
   - Check that featured events are displayed correctly
   - Verify stats are displayed correctly
   - Log in and verify that upcoming events are displayed correctly
3. Test navigation from the home page to other pages
4. Test authentication-specific features

## Troubleshooting Common Issues

### Database Connection Issues

- Verify the connection string in appsettings.json
- Ensure the SQLite database file exists and is accessible
- Check for any SQLite-specific syntax differences between the legacy and new applications

### View Component Issues

- If view components aren't rendering, check the component naming and view paths
- Ensure the view component classes are in the correct namespace
- Verify that the view component views are in the correct location

### Authentication Issues

- Check that the authentication cookie is properly configured
- Verify that User.Identity.IsAuthenticated works correctly
- Ensure claims are properly populated

## Next Steps After Migration

1. Consider refactoring the direct SQL queries to use a more structured approach
2. Implement proper error handling and logging
3. Add unit tests for the migrated components
4. Consider implementing a more robust repository pattern
5. Review and optimize database queries
