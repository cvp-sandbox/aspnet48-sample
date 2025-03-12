﻿using System.Collections.Generic;
using System.Web.Mvc;
using EventRegistrationSystem.Models;
using EventRegistrationSystem.Repositories;
using System.Data;
using EventRegistrationSystem.Data;
using Dapper;

namespace EventRegistrationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public HomeController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public ActionResult Index()
        {
            // Get featured events using the provided SQL query
            IEnumerable<Event> featuredEvents;
            
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                featuredEvents = connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate
                    LIMIT 3");
            }
            
            ViewBag.FeaturedEvents = featuredEvents;
            return View();
        }

        // This action method is kept for backward compatibility
        // The _Layout.cshtml now links directly to About.aspx
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult _FeaturedEvents()
        {
            // Get featured events using the provided SQL query
            IEnumerable<Event> featuredEvents;
            
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                featuredEvents = connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate
                    LIMIT 3");
            }
            
            return PartialView(featuredEvents);
        }

        public ActionResult _Stats()
        {
            // Get stats data using the provided SQL query
            dynamic stats;
            
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                stats = connection.Query(@"
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
            
            return PartialView(stats);
        }

        public ActionResult _UpcomingEvents()
        {
            // If user is not authenticated, return empty result
            if (!User.Identity.IsAuthenticated)
            {
                return PartialView(new List<Registration>());
            }

            // Get user's upcoming events using the provided SQL query
            IEnumerable<Registration> upcomingEvents;
            
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                upcomingEvents = connection.Query<Registration, Event, Registration>(@"
                    SELECT r.*, e.*
                    FROM Registrations r
                    JOIN Events e ON r.EventId = e.EventId
                    JOIN AspNetUsers u on u.Id = r.UserId
                    WHERE u.UserName = @UserName AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
                    ORDER BY e.EventDate DESC",
                    (registration, eventItem) => 
                    {
                        registration.Event = eventItem;
                        return registration;
                    },
                    new { UserName = User.Identity.Name },
                    splitOn: "EventId");
            }
            
            return PartialView(upcomingEvents);
        }
    }
}
