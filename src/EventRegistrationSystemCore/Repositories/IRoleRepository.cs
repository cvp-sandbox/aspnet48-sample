using EventRegistrationSystemCore.Models;

namespace EventRegistrationSystemCore.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<string> GetUserRoles(string userId);
        bool AddUserToRole(string userId, string roleName);
        bool RemoveUserFromRole(string userId, string roleName);
        bool UserIsInRole(string userId, string roleName);
    }
}
