using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Api.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> AddUserToRoleAsync(string userId, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<bool> UserIsInRoleAsync(string userId, string roleName);

        Task<bool> EnsureRoleExistsAsync(string roleName);
    }


}
