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
                
                return View(featuredEvents);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
