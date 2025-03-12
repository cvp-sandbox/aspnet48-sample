﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventRegistrationSystem.Data;
using EventRegistrationSystem.Models;
using Microsoft.AspNet.Identity;

namespace EventRegistrationSystem.Identity
{
    public class SqliteUserStore :
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>, 
        IUserLockoutStore<ApplicationUser, string>,
        IUserTwoFactorStore<ApplicationUser, string>
    {
        public Task CreateAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    connection.Execute(@"
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
                }
            });
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    connection.Execute(@"
                        UPDATE AspNetUsers SET
                        UserName = @UserName,
                        Email = @Email,
                        EmailConfirmed = @EmailConfirmed,
                        PasswordHash = @PasswordHash,
                        SecurityStamp = @SecurityStamp,
                        PhoneNumber = @PhoneNumber,
                        PhoneNumberConfirmed = @PhoneNumberConfirmed,
                        TwoFactorEnabled = @TwoFactorEnabled,
                        LockoutEndDateUtc = @LockoutEndDateUtc,
                        LockoutEnabled = @LockoutEnabled,
                        AccessFailedCount = @AccessFailedCount
                        WHERE Id = @Id",
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
                }
            });
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    connection.Execute("DELETE FROM AspNetUsers WHERE Id = @Id", new { Id = user.Id });
                }
            });
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    return connection.Query<ApplicationUser>(@"
                        SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                               PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                               LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
                        FROM AspNetUsers
                        WHERE Id = @Id", new { Id = userId })
                        .FirstOrDefault();
                }
            });
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    return connection.Query<ApplicationUser>(@"
                        SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                               PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                               LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
                        FROM AspNetUsers
                        WHERE LOWER(UserName) = LOWER(@UserName)", new { UserName = userName })
                        .FirstOrDefault();
                }
            });
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    return connection.Query<ApplicationUser>(@"
                        SELECT Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                               PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                               LockoutEndDateUtc, LockoutEnabled, AccessFailedCount
                        FROM AspNetUsers
                        WHERE LOWER(Email) = LOWER(@Email)", new { Email = email })
                        .FirstOrDefault();
                }
            });
        }

        public Task<int> GetUserCountAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connection = DatabaseConfig.GetConnection())
                {
                    connection.Open();
                    return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM AspNetUsers");
                }
            });
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            DateTimeOffset lockoutEnd = user.LockoutEndDateUtc.HasValue
                  ? new DateTimeOffset(user.LockoutEndDateUtc.Value)
                  : DateTimeOffset.MinValue;

            return Task.FromResult(lockoutEnd);
        }
        
        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = (lockoutEnd == DateTimeOffset.MinValue)
                ? (DateTime?)null
                : lockoutEnd.UtcDateTime;

            return Task.CompletedTask;
        }
       
        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            // Perform the reset operation (example)
            user.AccessFailedCount = 0;

            // Return a completed Task since no real async work is needed
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            // Perform the reset operation (example)
            user.AccessFailedCount = 0;

            // Return a completed Task since no real async work is needed
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(true);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            // Perform the reset operation (example)
            user.LockoutEnabled = false;

            // Return a completed Task since no real async work is needed
            return Task.CompletedTask;
        }

        // IUserTwoFactorStore implementation
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }


}
