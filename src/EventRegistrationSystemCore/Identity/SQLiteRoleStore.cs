using Microsoft.AspNetCore.Identity;
using System.Data;
using Dapper;

namespace EventRegistrationSystemCore.Identity;

public class SQLiteRoleStore : IRoleStore<IdentityRole>
{
    private readonly IDbConnection _connection;

    public SQLiteRoleStore(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = _connection)
        {
            connection.Open();
            var roleId = await connection.ExecuteScalarAsync<int>(
                "INSERT INTO Roles (Name) VALUES (@Name); SELECT last_insert_rowid();",
                new { Name = role.Name });

            role.Id = roleId.ToString();
            return IdentityResult.Success;
        }
    }

    public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = _connection)
        {
            connection.Open();
            await connection.ExecuteAsync(
                "DELETE FROM Roles WHERE RoleId = @RoleId",
                new { RoleId = int.Parse(role.Id) });

            return IdentityResult.Success;
        }
    }

public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

    using (var connection = _connection)
    {
        connection.Open();
        var role = await connection.QueryFirstOrDefaultAsync<IdentityRole>(
            "SELECT RoleId as Id, Name, NormalizedName FROM Roles WHERE RoleId = @RoleId",
            new { RoleId = int.Parse(roleId) });

        return role;
    }
}

public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

    using (var connection = _connection)
    {
        connection.Open();
        // Assuming Name is used as NormalizedName in your legacy app
        var role = await connection.QueryFirstOrDefaultAsync<IdentityRole>(
            "SELECT RoleId as Id, Name, Name as NormalizedName FROM Roles WHERE Name = @Name",
            new { Name = normalizedRoleName });

        return role;
    }
}

public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
{
    return Task.FromResult(role.Name); // Legacy app likely doesn't have normalized names
}

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
{
    return Task.FromResult(role.Name);
}

public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
{
    role.NormalizedName = normalizedName;
    return Task.CompletedTask; // We don't need to save this as legacy app doesn't use it
}

public async Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    role.Name = roleName ?? string.Empty;

    using (var connection = _connection)
    {
        connection.Open();
        await connection.ExecuteAsync(
            "UPDATE Roles SET Name = @Name WHERE RoleId = @RoleId",
            new { Name = roleName, RoleId = int.Parse(role.Id) });
    }
}

    public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = _connection)
        {
            connection.Open();
            await connection.ExecuteAsync(
                "UPDATE Roles SET Name = @Name WHERE RoleId = @RoleId",
                new { Name = role.Name, RoleId = int.Parse(role.Id) });

            return IdentityResult.Success;
        }
    }

    public void Dispose()
    {
        // IDbConnection is disposed in the individual methods
    }
}
