using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventManagementSystemv2.Models;
using EventManagementSystemv2.Repositories.Interfaces;

namespace EventManagementSystemv2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEventRepository eventRepository, ILogger<HomeController> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var featuredEvents = await _eventRepository.GetFeaturedEventsAsync();
                ViewBag.FeaturedEvents = featuredEvents;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving featured events for home page");
                return View(new List<Event>());
            }
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

        public async Task<IActionResult> FeaturedEvents()
        {
            try
            {
                var featuredEvents = await _eventRepository.GetFeaturedEventsAsync();
                return PartialView("_FeaturedEvents", featuredEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving featured events");
                return PartialView("_FeaturedEvents", new List<Event>());
            }
        }

        public async Task<IActionResult> Stats()
        {
            try
            {
                var stats = await _eventRepository.GetStatsAsync();
                return PartialView("_Stats", stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stats");
                return PartialView("_Stats", new List<dynamic>());
            }
        }

        public async Task<IActionResult> UpcomingEvents()
        {
            // If user is not authenticated, return empty result
            if (!User.Identity.IsAuthenticated)
            {
                return PartialView("_UpcomingEvents", new List<Registration>());
            }

            try
            {
                var upcomingEvents = await _eventRepository.GetUpcomingUserEventsAsync(User.Identity.Name);
                return PartialView("_UpcomingEvents", upcomingEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming events");
                return PartialView("_UpcomingEvents", new List<Registration>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
