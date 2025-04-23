using EventRegistrationSystemCore.Models;
using System.Collections.Generic;

namespace EventRegistrationSystemCore.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAllEvents();
        Event GetEventById(int eventId);
        IEnumerable<Event> GetUpcomingEvents(int count);
        IEnumerable<Event> GetFeaturedEvents(int count);
    }

    public interface IRegistrationRepository
    {
        IEnumerable<Registration> GetUserUpcomingEvents(string userId);
    }
}
