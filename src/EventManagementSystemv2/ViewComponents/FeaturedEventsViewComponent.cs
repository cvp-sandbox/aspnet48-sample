using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystemv2.Repositories.Interfaces;

namespace EventManagementSystemv2.ViewComponents
{
    public class FeaturedEventsViewComponent : ViewComponent
    {
        private readonly IEventRepository _eventRepository;

        public FeaturedEventsViewComponent(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var featuredEvents = await _eventRepository.GetFeaturedEventsAsync();
            return View("_FeaturedEvents", featuredEvents);
        }
    }
}
