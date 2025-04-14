using EventRegistrationSystemCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Dapper;
using System.Security.Claims;

namespace EventRegistrationSystemCore.Identity;

public class SQLiteUserStore :
    IUserStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>,
    IUserEmailStore<ApplicationUser>,
    IUserLockoutStore<ApplicationUser>,
    IUserTwoFactorStore<ApplicationUser>
{

    private readonly IDbConnection _connection;

    public SQLiteUserStore(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        using var connection = _connection;
        connection.Open();
        await connection.ExecuteAsync(@"
                        INSERT INTO AspNetUsers
                        (Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                         PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                         LockoutEndDateUtc, LockoutEnabled, AccessFailedCount)
                        VALUES
                        (@Id, @UserName, @Email, @EmailConfirmed, @PasswordHash, @SecurityStamp,
                         @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, 
                         @LockoutEndDateUtc, @LockoutEnabled, @AccessFailedCount)",
         new
         {
             Id = user.Id,
             UserName = user.UserName,
             Email = user.Email,
             EmailConfirmed = user.EmailConfirmed ? 1 : 0,
             PasswordHash = user.PasswordHash,
             SecurityStamp = user.SecurityStamp,
             PhoneNumber = user.PhoneNumber,
             PhoneNumberConfirmed = user.PhoneNumberConfirmed ? 1 : 0,
             TwoFactorEnabled = user.TwoFactorEnabled ? 1 : 0,
             LockoutEndDateUtc = user.LockoutEndDateUtc,
             LockoutEnabled = user.LockoutEnabled ? 1 : 0,
             AccessFailedCount = user.AccessFailedCount
         });
        return IdentityResult.Success; // Add appropriate return value
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        //implement delete logic
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        using var connection = _connection;
        connection.Open();
        await connection.ExecuteAsync("DELETE FROM AspNetUsers WHERE Id = @Id", new { Id = user.Id });
        return IdentityResult.Success; // Add appropriate return value
    }

    public void Dispose()
    {
        // Nothing to dispose
    }





    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        using var connection = _connection;
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(@"
                        SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                               PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                               LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
                        FROM AspNetUsers
                        WHERE Id = @Id", new { Id = userId }) ;

    }

    public async Task<ApplicationUser?> FindByNameAsync(string userName, CancellationToken cancellationToken)
    {

        using var connection = _connection;
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(@"
                        SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                               PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                               LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
                        FROM AspNetUsers
                        WHERE LOWER(UserName) = LOWER(@UserName)", new { UserName = userName });

    }



    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        using var connection = _connection;
        connection.Open();
        var roles = await connection.QueryAsync<string>(@"
                SELECT r.Name 
                FROM Roles r
                JOIN UserRoles ur ON r.RoleId = ur.RoleId
                WHERE ur.UserId = @UserId",
            new { UserId = userId });
        return roles;


    }

    // Add implementations for IUserLockoutStore and IUserTwoFactorStore
    public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        if (user.LockoutEndDateUtc.HasValue)
        {
            return Task.FromResult<DateTimeOffset?>(new DateTimeOffset(user.LockoutEndDateUtc.Value));
        }
        return Task.FromResult<DateTimeOffset?>(null);
    }

    public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
        return Task.CompletedTask;
    }

    public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    // Make sure to implement save methods that actually update the database
    // when properties like LockoutEnabled, etc. are changed

    // Additional IUserEmailStore implementation
    public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        using var connection = _connection;
        connection.Open();
        var result = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(
            "SELECT * FROM AspNetUsers WHERE Email = @Email",
            new { Email = normalizedEmail });
        return result;
    }

    public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email); // Legacy app likely doesn't normalize
    }

    public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        // Legacy app doesn't use normalized values, so just store it in memory
        return Task.CompletedTask;
    }


    // Implement other IUserStore and IUserPasswordStore methods
    // Use the same SQL queries as in your legacy app
}

