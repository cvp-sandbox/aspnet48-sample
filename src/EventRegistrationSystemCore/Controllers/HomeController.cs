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
        if (User.Identity == null || !User.Identity.IsAuthenticated)
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
