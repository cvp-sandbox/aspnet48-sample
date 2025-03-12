using System.Collections.Generic;
using EventRegistrationSystem.Authorization;
using EventRegistrationSystem.Models;

namespace EventRegistrationSystem.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAllEvents();
        Event GetEventById(int eventId);
        IEnumerable<Event> GetEventsByCreator(string userId);
        int CreateEvent(Event eventEntity);
        bool UpdateEvent(Event eventEntity);
        bool DeleteEvent(int eventId);
        int GetRegistrationCount(int eventId);
    }

    public interface IRegistrationRepository
    {
        IEnumerable<Registration> GetRegistrationsByEventId(int eventId);
        IEnumerable<Registration> GetRegistrationsByUserId(string userId);
        Registration GetRegistration(int eventId, string userId);
        int CreateRegistration(Registration registration);
        bool CancelRegistration(int eventId, string userId);
    }

    public interface IRoleRepository
    {
        IEnumerable<UserRole> GetUserRoles(string userId);
        bool AddUserToRole(string userId, string roleName);
        bool RemoveUserFromRole(string userId, string roleName);
        bool UserIsInRole(string userId, string roleName);
    }
}