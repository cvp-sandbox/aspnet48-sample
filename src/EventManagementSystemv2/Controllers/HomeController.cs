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

        public IActionResult Index()
        {
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

        // Removed partial view action methods as they're now handled by ViewComponents

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
