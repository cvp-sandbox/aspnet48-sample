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
        public ActionResult Details(int id)
        {
            var eventEntity = _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();
            ViewBag.IsRegistered = _registrationRepository.GetRegistration(id, userId) != null;
            ViewBag.IsCreator = eventEntity.CreatedBy == userId;

            return View(eventEntity);
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
        public ActionResult Create(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                eventEntity.CreatedBy = User.Identity.GetUserId();
                eventEntity.CreatedDate = DateTime.Now;

                int eventId = _eventRepository.CreateEvent(eventEntity);
                return RedirectToAction("Details", new { id = eventId });
            }

            return View(eventEntity);
        }

        // GET: Event/Edit/5
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public ActionResult Edit(int id)
        {
            var eventEntity = _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return HttpNotFound();
            }

            // Only event creator or admin can edit
            string userId = User.Identity.GetUserId();
            if (eventEntity.CreatedBy != userId && !User.IsInRole(Roles.Admin))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            return View(eventEntity);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public ActionResult Edit(Event eventEntity)
        {
            if (ModelState.IsValid)
            {
                var originalEvent = _eventRepository.GetEventById(eventEntity.EventId);

                // Only event creator or admin can edit
                string userId = User.Identity.GetUserId();
                if (originalEvent.CreatedBy != userId && !User.IsInRole(Roles.Admin))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }

                _eventRepository.UpdateEvent(eventEntity);
                return RedirectToAction("Details", new { id = eventEntity.EventId });
            }

            return View(eventEntity);
        }

        // GET: Event/Delete/5
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public ActionResult Delete(int id)
        {
            var eventEntity = _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return HttpNotFound();
            }

            // Only event creator or admin can delete
            string userId = User.Identity.GetUserId();
            if (eventEntity.CreatedBy != userId && !User.IsInRole(Roles.Admin))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            return View(eventEntity);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
        public ActionResult DeleteConfirmed(int id)
        {
            var eventEntity = _eventRepository.GetEventById(id);

            // Only event creator or admin can delete
            string userId = User.Identity.GetUserId();
            if (eventEntity.CreatedBy != userId && !User.IsInRole(Roles.Admin))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            _eventRepository.DeleteEvent(id);
            return RedirectToAction("Index");
        }

        // POST: Event/Register/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(int id)
        {
            var eventEntity = _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return HttpNotFound();
            }

            // Check if event is full
            int registrationCount = _eventRepository.GetRegistrationCount(id);
            if (eventEntity.MaxAttendees > 0 && registrationCount >= eventEntity.MaxAttendees)
            {
                TempData["ErrorMessage"] = "This event is already at full capacity.";
                return RedirectToAction("Details", new { id });
            }

            string userId = User.Identity.GetUserId();

            // Check if already registered
            if (_registrationRepository.GetRegistration(id, userId) != null)
            {
                TempData["ErrorMessage"] = "You are already registered for this event.";
                return RedirectToAction("Details", new { id });
            }

            var registration = new Registration
            {
                EventId = id,
                UserId = userId,
                RegistrationDate = DateTime.Now
            };

            _registrationRepository.CreateRegistration(registration);
            TempData["SuccessMessage"] = "You have successfully registered for this event.";

            return RedirectToAction("Details", new { id });
        }

        // POST: Event/CancelRegistration/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRegistration(int id)
        {
            string userId = User.Identity.GetUserId();

            // Check if registered
            if (_registrationRepository.GetRegistration(id, userId) == null)
            {
                TempData["ErrorMessage"] = "You are not registered for this event.";
                return RedirectToAction("Details", new { id });
            }

            _registrationRepository.CancelRegistration(id, userId);
            TempData["SuccessMessage"] = "Your registration has been canceled.";

            return RedirectToAction("Details", new { id });
        }

        // GET: Event/MyEvents
        public ActionResult MyEvents()
        {
            string userId = User.Identity.GetUserId();
            var events = _eventRepository.GetEventsByCreator(userId);
            return View(events);
        }

        // GET: Event/MyRegistrations
        public ActionResult MyRegistrations()
        {
            string userId = User.Identity.GetUserId();
            var registrations = _registrationRepository.GetRegistrationsByUserId(userId);
            return View(registrations);
        }
    }
}