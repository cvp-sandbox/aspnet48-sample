using EventManagement.Api.Models;

namespace EventManagement.Api.Repositories;

public interface IRegistrationRepository
{
    Task<IEnumerable<Registration>> GetRegistrationsByEventIdAsync(int eventId);
    Task<IEnumerable<Registration>> GetRegistrationsByUserIdAsync(string userId);
    Task<Registration?> GetRegistrationAsync(int eventId, string userId);
    Task<int> CreateRegistrationAsync(Registration registration);
    Task<bool> CancelRegistrationAsync(int eventId, string userId);
}
