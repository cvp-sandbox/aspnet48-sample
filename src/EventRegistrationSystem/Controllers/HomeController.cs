﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EventRegistrationSystem.Models;
using EventRegistrationSystem.Models.Api;
using EventRegistrationSystem.Repositories;
using EventRegistrationSystem.Utils;
using Microsoft.AspNet.Identity;
using EventRegistrationSystem.Authorization;
using EventRegistrationSystem.Data;
using Dapper;

namespace EventRegistrationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IApiClient _apiClient;

        public HomeController(IEventRepository eventRepository, IApiClient apiClient)
        {
            _eventRepository = eventRepository;
            _apiClient = apiClient;
        }

        public async Task<ActionResult> Index()
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            // Call the API
            var response = await _apiClient.GetAsync<GetFeaturedEventsResponse>(
                "api/events/featured",
                username,
                roles.ToArray());

            ViewBag.FeaturedEvents = response.Events;
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

        [ChildActionOnly]
        public ActionResult _FeaturedEvents()
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            // Call the API synchronously
            var task = _apiClient.GetAsync<GetFeaturedEventsResponse>(
                "api/events/featured",
                username,
                roles.ToArray());
            
            // Wait for the result - this is not ideal but necessary for child actions
            task.Wait();
            var response = task.Result;
            
            return PartialView(response.Events);
        }

        [ChildActionOnly]
        public ActionResult _Stats()
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            // Call the API synchronously
            var task = _apiClient.GetAsync<GetStatsResponse>(
                "api/events/stats",
                username,
                roles.ToArray());
            
            // Wait for the result - this is not ideal but necessary for child actions
            task.Wait();
            var response = task.Result;
            
            return PartialView(response.Stats);
        }

        [ChildActionOnly]
        public ActionResult _UpcomingEvents()
        {
            // If user is not authenticated, return empty result
            if (!User.Identity.IsAuthenticated)
            {
                return PartialView(new List<Registration>());
            }

            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            // Call the API synchronously
            var task = _apiClient.GetAsync<GetUpcomingEventsResponse>(
                "api/events/upcoming",
                username,
                roles.ToArray());
            
            // Wait for the result - this is not ideal but necessary for child actions
            task.Wait();
            var response = task.Result;
            
            // Convert API model to view model
            var registrations = response.Registrations.Select(r => new Registration
            {
                RegistrationId = r.RegistrationId,
                EventId = r.EventId,
                UserId = r.UserId,
                RegistrationDate = r.RegistrationDate,
                UserName = r.UserName,
                Event = r.Event
            }).ToList();
            
            return PartialView(registrations);
        }
    }
}
