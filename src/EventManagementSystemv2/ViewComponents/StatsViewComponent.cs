using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystemv2.Repositories.Interfaces;

namespace EventManagementSystemv2.ViewComponents
{
    public class StatsViewComponent : ViewComponent
    {
        private readonly IEventRepository _eventRepository;

        public StatsViewComponent(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = await _eventRepository.GetStatsAsync();
            return View("_Stats", stats);
        }
    }
}
