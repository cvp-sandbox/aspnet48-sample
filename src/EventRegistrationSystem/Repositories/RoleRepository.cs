using System.Collections.Generic;
using Dapper;
using EventRegistrationSystem.Authorization;
using EventRegistrationSystem.Data;

namespace EventRegistrationSystem.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public IEnumerable<UserRole> GetUserRoles(string userId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                return connection.Query<UserRole>(@"
                    SELECT ur.*, r.Name as RoleName
                    FROM UserRoles ur
                    JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = @UserId",
                    new { UserId = userId });
            }
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();

                // Get the role ID
                int? roleId = connection.ExecuteScalar<int?>(
                    "SELECT RoleId FROM Roles WHERE Name = @RoleName",
                    new { RoleName = roleName });

                if (!roleId.HasValue)
                    return false;

                // Check if user is already in the role
                if (UserIsInRole(userId, roleName))
                    return true;

                // Add user to role
                int rowsAffected = connection.Execute(@"
                    INSERT INTO UserRoles (UserId, RoleId)
                    VALUES (@UserId, @RoleId)",
                    new { UserId = userId, RoleId = roleId.Value });

                return rowsAffected > 0;
            }
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();

                // Get the role ID
                int? roleId = connection.ExecuteScalar<int?>(
                    "SELECT RoleId FROM Roles WHERE Name = @RoleName",
                    new { RoleName = roleName });

                if (!roleId.HasValue)
                    return false;

                // Remove user from role
                int rowsAffected = connection.Execute(@"
                    DELETE FROM UserRoles 
                    WHERE UserId = @UserId AND RoleId = @RoleId",
                    new { UserId = userId, RoleId = roleId.Value });

                return rowsAffected > 0;
            }
        }

        public bool UserIsInRole(string userId, string roleName)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(@"
                    SELECT COUNT(*) 
                    FROM UserRoles ur
                    JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = @UserId AND r.Name = @RoleName",
                    new { UserId = userId, RoleName = roleName });

                return count > 0;
            }
        }
    }
}