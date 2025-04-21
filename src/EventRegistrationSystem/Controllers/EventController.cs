using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using EventRegistrationSystem.Attributes;
using EventRegistrationSystem.Authorization;
using EventRegistrationSystem.Models;
using EventRegistrationSystem.Models.Api;
using EventRegistrationSystem.Repositories;
using EventRegistrationSystem.Utils;
using Microsoft.AspNet.Identity;

namespace EventRegistrationSystem.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRegistrationRepository _registrationRepository;

        private readonly IApiClient _apiClient;


        public EventController(
            
            IEventRepository eventRepository,
            IRegistrationRepository registrationRepository,
            IApiClient apiClient)
        {
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
            _apiClient = apiClient;

        }

        // GET: Event
        public async Task<ActionResult> Index()
        {
            //var events = _eventRepository.GetAllEvents();
            //return View(events);

            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            // Call the API
            var response = await _apiClient.GetAsync<GetAllEventsResponse>(
                "api/events",
                username,
                roles.ToArray());

            return View(response.Events);
        }

        // GET: Event/Details/5
        public async Task<ActionResult> Details(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.GetAsync<dynamic>(
                    $"api/events/{id}",
                    username,
                    roles.ToArray());

                if (response.Event == null)
                {
                    return HttpNotFound();
                }

                // Convert the dynamic response to an Event object
                var eventEntity = new Event
                {
                    EventId = response.Event.EventId,
                    Name = response.Event.Name,
                    Description = response.Event.Description,
                    EventDate = response.Event.EventDate,
                    Location = response.Event.Location,
                    MaxAttendees = response.Event.MaxAttendees,
                    CreatedBy = response.Event.CreatedBy,
                    CreatedDate = response.Event.CreatedDate,
                    RegistrationCount = response.RegistrationCount
                };

                ViewBag.IsRegistered = response.IsRegistered;
                ViewBag.IsCreator = response.IsCreator;

                return View(eventEntity);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // GET: Event/Create
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public async Task<ActionResult> Create(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.GetUserName();

                // Get user roles
                var roles = new List<string>();
                if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
                if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
                if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

                try
                {
                    // Prepare request data
                    var requestData = new
                    {
                        Name = eventEntity.Name,
                        Description = eventEntity.Description,
                        EventDate = eventEntity.EventDate,
                        Location = eventEntity.Location,
                        MaxAttendees = eventEntity.MaxAttendees
                    };

                    // Call the API
                    var response = await _apiClient.PostAsync<object, dynamic>(
                        "api/events",
                        requestData,
                        username,
                        roles.ToArray());

                    return RedirectToAction("Details", new { id = response.EventId });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "An error occurred while creating the event.");
                }
            }

            return View(eventEntity);
        }

        // GET: Event/Edit/5
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public async Task<ActionResult> Edit(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.GetAsync<dynamic>(
                    $"api/events/{id}",
                    username,
                    roles.ToArray());

                if (response.Event == null)
                {
                    return HttpNotFound();
                }

                // Check if user is authorized to edit
                if (!response.IsCreator && !User.IsInRole(Roles.Admin))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }

                // Convert the dynamic response to an Event object
                var eventEntity = new Event
                {
                    EventId = response.Event.EventId,
                    Name = response.Event.Name,
                    Description = response.Event.Description,
                    EventDate = response.Event.EventDate,
                    Location = response.Event.Location,
                    MaxAttendees = response.Event.MaxAttendees,
                    CreatedBy = response.Event.CreatedBy,
                    CreatedDate = response.Event.CreatedDate
                };

                return View(eventEntity);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public async Task<ActionResult> Edit(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                string username = User.Identity.GetUserName();

                // Get user roles
                var roles = new List<string>();
                if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
                if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
                if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

                try
                {
                    // Prepare request data
                    var requestData = new
                    {
                        EventId = eventEntity.EventId,
                        Name = eventEntity.Name,
                        Description = eventEntity.Description,
                        EventDate = eventEntity.EventDate,
                        Location = eventEntity.Location,
                        MaxAttendees = eventEntity.MaxAttendees
                    };

                    // Call the API
                    await _apiClient.PostAsync<object, dynamic>(
                        $"api/events/{eventEntity.EventId}",
                        requestData,
                        username,
                        roles.ToArray(),
                        HttpMethod.Put);

                    return RedirectToAction("Details", new { id = eventEntity.EventId });
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403"))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "An error occurred while updating the event.");
                }
            }

            return View(eventEntity);
        }

        // GET: Event/Delete/5
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public async Task<ActionResult> Delete(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.GetAsync<dynamic>(
                    $"api/events/{id}",
                    username,
                    roles.ToArray());

                if (response.Event == null)
                {
                    return HttpNotFound();
                }

                // Check if user is authorized to delete
                if (!response.IsCreator && !User.IsInRole(Roles.Admin))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }

                // Convert the dynamic response to an Event object
                var eventEntity = new Event
                {
                    EventId = response.Event.EventId,
                    Name = response.Event.Name,
                    Description = response.Event.Description,
                    EventDate = response.Event.EventDate,
                    Location = response.Event.Location,
                    MaxAttendees = response.Event.MaxAttendees,
                    CreatedBy = response.Event.CreatedBy,
                    CreatedDate = response.Event.CreatedDate
                };

                return View(eventEntity);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                await _apiClient.DeleteAsync(
                    $"api/events/{id}",
                    username,
                    roles.ToArray());

                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("403"))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // POST: Event/Register/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.PostAsync<object, dynamic>(
                    $"api/events/{id}/register",
                    null,
                    username,
                    roles.ToArray());

                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                }

                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = "An error occurred while registering for the event.";
                return RedirectToAction("Details", new { id });
            }
        }

        // POST: Event/CancelRegistration/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelRegistration(int id)
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.PostAsync<object, dynamic>(
                    $"api/events/{id}/cancel-registration",
                    null,
                    username,
                    roles.ToArray());

                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                }

                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = "An error occurred while canceling your registration.";
                return RedirectToAction("Details", new { id });
            }
        }

        // GET: Event/MyEvents
        public async Task<ActionResult> MyEvents()
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.GetAsync<dynamic>(
                    "api/events/my-events",
                    username,
                    roles.ToArray());

                // Convert the dynamic response to a list of Event objects
                var events = new List<Event>();
                if (response.Events == null)
                {
                    return View(events);
                }


                foreach (var item in response.Events)
                {
                    events.Add(new Event
                    {
                        EventId = item.EventId,
                        Name = item.Name,
                        Description = item.Description,
                        EventDate = item.EventDate,
                        Location = item.Location,
                        MaxAttendees = item.MaxAttendees,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    });
                }

                return View(events);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // GET: Event/MyRegistrations
        public async Task<ActionResult> MyRegistrations()
        {
            string username = User.Identity.GetUserName();

            // Get user roles
            var roles = new List<string>();
            if (User.IsInRole(Roles.Admin)) roles.Add(Roles.Admin);
            if (User.IsInRole(Roles.Organizer)) roles.Add(Roles.Organizer);
            if (User.IsInRole(Roles.User)) roles.Add(Roles.User);

            try
            {
                // Call the API
                var response = await _apiClient.GetAsync<dynamic>(
                    "api/events/my-registrations",
                    username,
                    roles.ToArray());

                // Convert the dynamic response to a list of Registration objects
                var registrations = new List<Registration>();
                if (response.Registrations == null)
                {
                    return View(registrations);
                }
                foreach (var item in response.Registrations)
                {
                    var registration = new Registration
                    {
                        RegistrationId = item.RegistrationId,
                        EventId = item.EventId,
                        UserId = item.UserId,
                        RegistrationDate = item.RegistrationDate
                    };

                    if (item.Event != null)
                    {
                        registration.Event = new Event
                        {
                            EventId = item.Event.EventId,
                            Name = item.Event.Name,
                            Description = item.Event.Description,
                            EventDate = item.Event.EventDate,
                            Location = item.Event.Location,
                            MaxAttendees = item.Event.MaxAttendees,
                            CreatedBy = item.Event.CreatedBy,
                            CreatedDate = item.Event.CreatedDate
                        };
                    }

                    registrations.Add(registration);
                }

                return View(registrations);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }
    }
}
