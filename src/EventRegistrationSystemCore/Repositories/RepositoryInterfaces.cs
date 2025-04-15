using EventRegistrationSystemCore.Models;

namespace EventRegistrationSystemCore.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetUpcomingEvents(int count);
        IEnumerable<Event> GetFeaturedEvents(int count);
    }

    public interface IRegistrationRepository
    {
        IEnumerable<Registration> GetUserUpcomingEvents(string userId);
    }
}
