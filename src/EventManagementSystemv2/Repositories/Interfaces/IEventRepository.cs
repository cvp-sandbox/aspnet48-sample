using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystemv2.Models;

namespace EventManagementSystemv2.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetEventsByCreatorAsync(string userId);
        Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 3);
        Task<int> CreateEventAsync(Event eventEntity);
        Task<bool> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int eventId);
        Task<int> GetRegistrationCountAsync(int eventId);
        Task<IEnumerable<dynamic>> GetStatsAsync();
        Task<IEnumerable<Registration>> GetUpcomingUserEventsAsync(string userName);
    }
}
