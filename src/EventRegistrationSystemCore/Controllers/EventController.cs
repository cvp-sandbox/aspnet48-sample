using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventRegistrationSystemCore.Models;
using EventRegistrationSystemCore.Repositories;
using System.Collections.Generic;

namespace EventRegistrationSystemCore.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventRepository eventRepository, ILogger<EventController> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        // GET: Event
        public IActionResult Index()
        {
            _logger.LogInformation("Retrieving all events");
            var events = _eventRepository.GetAllEvents();
            return View(events);
        }
    }
}
