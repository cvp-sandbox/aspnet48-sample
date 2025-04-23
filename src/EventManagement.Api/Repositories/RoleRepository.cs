using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EventManagement.Api.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _connection;

        public RoleRepository(IDbConnection connection)
        {
            _connection = connection;
        }




        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            const string sql = @"
                SELECT r.Name
                FROM Roles r
                INNER JOIN UserRoles ur ON r.RoleId = ur.RoleId
                WHERE ur.UserId = @UserId";

            try
            {
                var roles = await _connection.QueryAsync<string>(sql, new { UserId = userId });
                return roles ?? Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user roles: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            // First, ensure the role exists
            await EnsureRoleExistsAsync(roleName);

            // Get the role ID
            const string getRoleIdSql = @"
                SELECT RoleId 
                FROM Roles 
                WHERE Name = @Name";

            int? roleId = await _connection.QuerySingleOrDefaultAsync<int?>(
                getRoleIdSql,
                new { Name = roleName });

            if (roleId == null)
            {
                return false;
            }

            // Check if the user is already in the role
            const string checkUserRoleSql = @"
                SELECT COUNT(1) 
                FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";

            int count = await _connection.ExecuteScalarAsync<int>(
                checkUserRoleSql,
                new { UserId = userId, RoleId = roleId });

            if (count > 0)
            {
                // User is already in the role
                return true;
            }

            // Add user to role
            const string addUserToRoleSql = @"
                INSERT INTO UserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId)";

            try
            {
                int rowsAffected = await _connection.ExecuteAsync(
                    addUserToRoleSql,
                    new { UserId = userId, RoleId = roleId });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user to role: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            using var connection = _connection;

            // Get the role ID
            var roleId = await connection.QueryFirstOrDefaultAsync<string>(
                "SELECT Id FROM AspNetRoles WHERE Name = @RoleName",
                new { RoleName = roleName });

            if (string.IsNullOrEmpty(roleId))
            {
                return false; // Role doesn't exist
            }

            // Remove user from role
            var result = await connection.ExecuteAsync(
                "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                new { UserId = userId, RoleId = roleId });

            return result > 0;
        }

        public async Task<bool> UserIsInRoleAsync(string userId, string roleName)
        {
            using var connection = _connection;

            var count = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                  FROM AspNetUserRoles ur
                  INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                  WHERE ur.UserId = @UserId AND r.Name = @RoleName",
                new { UserId = userId, RoleName = roleName });

            return count > 0;
        }

        public async Task<bool> EnsureRoleExistsAsync(string roleName)
        {
            // Check if the role already exists
            const string checkRoleSql = @"
                SELECT COUNT(1) 
                FROM Roles 
                WHERE Name = @Name";

            int count = await _connection.ExecuteScalarAsync<int>(
                checkRoleSql,
                new { Name = roleName });

            if (count > 0)
            {
                // Role already exists
                return true;
            }

            // Create the role
            const string createRoleSql = @"
                INSERT INTO Roles (Name)
                VALUES (@Name)";

            try
            {
                int rowsAffected = await _connection.ExecuteAsync(createRoleSql, new
                {
                    Name = roleName
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating role: {ex.Message}");
                return false;
            }
        }
    }
}
