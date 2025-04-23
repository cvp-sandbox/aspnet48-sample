using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using EventManagement.Api.Common.Identity;

namespace EventManagement.Api.Repositories
{


    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly LegacyCompatiblePasswordHasher<IdentityUser> _passwordHasher;

        public UserRepository(
            IDbConnection dbConnection,
            LegacyCompatiblePasswordHasher<IdentityUser> passwordHasher)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM AspNetUsers 
                WHERE Email = @Email";

            int count = await _dbConnection.ExecuteScalarAsync<int>(
                sql, 
                new { Email = email });
                
            return count > 0;
        }

        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            const string sql = @"
                SELECT Id 
                FROM AspNetUsers 
                WHERE Email = @Email";

            return await _dbConnection.QuerySingleOrDefaultAsync<string>(
                sql, 
                new { Email = email });
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            const string sql = @"
                SELECT Id, PasswordHash 
                FROM AspNetUsers 
                WHERE Email = @Email";

            var user = await _dbConnection.QuerySingleOrDefaultAsync<UserRecord>(
                sql, 
                new { Email = email });

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return false;
            }

            // Use the custom password hasher to verify the password
            var result = _passwordHasher.VerifyHashedPassword(
                new IdentityUser { Id = user.Id }, 
                user.PasswordHash, 
                password);

            // If password verification was successful but hash needs to be updated
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Generate a new hash with the modern algorithm
                string newHash = _passwordHasher.HashPassword(new IdentityUser { Id = user.Id }, password);
                
                // Update the hash in the database
                await UpdatePasswordHashAsync(user.Id, newHash);
            }

            return result != PasswordVerificationResult.Failed;
        }

        public async Task<bool> CreateUserAsync(string email, string password)
        {
            // Generate a new user ID
            string userId = Guid.NewGuid().ToString();
            
            // Hash the password using the modern algorithm
            string passwordHash = _passwordHasher.HashPassword(new IdentityUser { Id = userId }, password);
            
            const string sql = @"
                INSERT INTO AspNetUsers (
                    Id, 
                    UserName, 
                    Email, 
                    EmailConfirmed, 
                    PasswordHash, 
                    SecurityStamp, 
                    PhoneNumber, 
                    PhoneNumberConfirmed, 
                    TwoFactorEnabled, 
                    LockoutEnabled, 
                    AccessFailedCount
                ) VALUES (
                    @Id, 
                    @Email, 
                    @Email, 
                    1, 
                    @PasswordHash, 
                    @SecurityStamp, 
                    NULL, 
                    0, 
                    0, 
                    1, 
                    0
                )";

            try
            {
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new
                {
                    Id = userId,
                    Email = email,
                    PasswordHash = passwordHash,
                    SecurityStamp = Guid.NewGuid().ToString()
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePasswordHashAsync(string userId, string newPasswordHash)
        {
            const string sql = @"
                UPDATE AspNetUsers 
                SET PasswordHash = @PasswordHash, 
                    SecurityStamp = @SecurityStamp
                WHERE Id = @Id";

            try
            {
                int rowsAffected = await _dbConnection.ExecuteAsync(sql, new
                {
                    Id = userId,
                    PasswordHash = newPasswordHash,
                    SecurityStamp = Guid.NewGuid().ToString()
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password hash: {ex.Message}");
                return false;
            }
        }

        // Helper class for retrieving user data
        public class UserRecord
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
        }
    }
}