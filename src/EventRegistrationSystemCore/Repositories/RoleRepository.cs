using Dapper;
using System.Data;

namespace EventRegistrationSystemCore.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _connection;

        public RoleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<string> GetUserRoles(string userId)
        {
            _connection.Open();
            try
            {
                return _connection.Query<string>(@"
                    SELECT r.Name
                    FROM UserRoles ur
                    JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = @UserId",
                    new { UserId = userId });
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            _connection.Open();
            try
            {
                // Get the role ID
                int? roleId = _connection.ExecuteScalar<int?>(
                    "SELECT RoleId FROM Roles WHERE Name = @RoleName",
                    new { RoleName = roleName });

                if (!roleId.HasValue)
                    return false;

                // Check if user is already in the role
                if (UserIsInRole(userId, roleName))
                    return true;

                // Add user to role
                int rowsAffected = _connection.Execute(@"
                    INSERT INTO UserRoles (UserId, RoleId)
                    VALUES (@UserId, @RoleId)",
                    new { UserId = userId, RoleId = roleId.Value });

                return rowsAffected > 0;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            _connection.Open();
            try
            {
                // Get the role ID
                int? roleId = _connection.ExecuteScalar<int?>(
                    "SELECT RoleId FROM Roles WHERE Name = @RoleName",
                    new { RoleName = roleName });

                if (!roleId.HasValue)
                    return false;

                // Remove user from role
                int rowsAffected = _connection.Execute(@"
                    DELETE FROM UserRoles 
                    WHERE UserId = @UserId AND RoleId = @RoleId",
                    new { UserId = userId, RoleId = roleId.Value });

                return rowsAffected > 0;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool UserIsInRole(string userId, string roleName)
        {
            _connection.Open();
            try
            {
                int count = _connection.ExecuteScalar<int>(@"
                    SELECT COUNT(*) 
                    FROM UserRoles ur
                    JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = @UserId AND r.Name = @RoleName",
                    new { UserId = userId, RoleName = roleName });

                return count > 0;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
