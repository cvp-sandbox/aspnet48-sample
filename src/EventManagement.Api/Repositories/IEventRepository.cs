using EventManagement.Api.Models;

namespace EventManagement.Api.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int eventId);
    Task<IEnumerable<Event>> GetEventsByCreatorAsync(string userId);
    Task<int> CreateEventAsync(Event eventEntity);
    Task<bool> UpdateEventAsync(Event eventEntity);
    Task<bool> DeleteEventAsync(int eventId);
    Task<int> GetRegistrationCountAsync(int eventId);
}