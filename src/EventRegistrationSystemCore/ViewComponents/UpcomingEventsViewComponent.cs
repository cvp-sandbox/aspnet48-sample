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
            if (HttpContext.User.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new List<Registration>());
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
                
                return View(upcomingEvents);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
