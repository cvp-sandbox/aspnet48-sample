using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystemv2.Models;
using EventManagementSystemv2.Repositories.Interfaces;

namespace EventManagementSystemv2.ViewComponents
{
    public class UpcomingEventsViewComponent : ViewComponent
    {
        private readonly IEventRepository _eventRepository;

        public UpcomingEventsViewComponent(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("_UpcomingEvents", new List<Registration>());
            }

            var upcomingEvents = await _eventRepository.GetUpcomingUserEventsAsync(User.Identity.Name);
            return View("_UpcomingEvents", upcomingEvents);
        }
    }
}
