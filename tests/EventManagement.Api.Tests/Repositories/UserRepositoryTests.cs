using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Api.Common.Identity;
using EventManagement.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Xunit;

namespace EventManagement.Api.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _dbConnection;
        private readonly LegacyCompatiblePasswordHasher<IdentityUser> _passwordHasher;
        private readonly UserRepository _repository;
        private readonly string _existingUserId = "user123";
        private readonly string _legacyUserId = "legacy123";
        private readonly string _existingEmail = "existing@example.com";
        private readonly string _legacyEmail = "legacy@example.com";
        private readonly string _validPassword = "Password123!";

        public UserRepositoryTests()
        {
            // Create and open the in-memory SQLite connection
            _dbConnection = new SqliteConnection("Data Source=:memory:");
            _dbConnection.Open();
            
            // Create the AspNetUsers table
            ExecuteNonQuery(@"
                CREATE TABLE AspNetUsers (
                    Id TEXT PRIMARY KEY,
                    UserName TEXT NOT NULL UNIQUE,
                    Email TEXT,
                    EmailConfirmed INTEGER,
                    PasswordHash TEXT,
                    SecurityStamp TEXT,
                    PhoneNumber TEXT,
                    PhoneNumberConfirmed INTEGER,
                    TwoFactorEnabled INTEGER,
                    LockoutEndDateUtc TEXT,
                    LockoutEnabled INTEGER,
                    AccessFailedCount INTEGER
                )");
            
            // Create the password hasher
            _passwordHasher = new LegacyCompatiblePasswordHasher<IdentityUser>();
            
            // Create the repository
            _repository = new UserRepository(_dbConnection, _passwordHasher);
            
            // Insert test users
            InsertTestUsers();
        }

        private void ExecuteNonQuery(string sql, object parameters = null)
        {
            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;
            
            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"@{prop.Name}";
                    param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                    cmd.Parameters.Add(param);
                }
            }
            
            cmd.ExecuteNonQuery();
        }
        
        private void InsertTestUsers()
        {
            // Create a user with a modern ASP.NET Core Identity hash
            var user = new IdentityUser { Id = _existingUserId };
            string passwordHash = _passwordHasher.HashPassword(user, _validPassword);
            
            ExecuteNonQuery(@"
                INSERT INTO AspNetUsers (
                    Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                    PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount
                ) VALUES (
                    @Id, @Email, @Email, 1, @PasswordHash, @SecurityStamp,
                    0, 0, 1, 0
                )",
                new { 
                    Id = _existingUserId, 
                    Email = _existingEmail, 
                    PasswordHash = passwordHash, 
                    SecurityStamp = Guid.NewGuid().ToString() 
                });
            
            // Create a user with a legacy hash format
            string legacyHash = "legacyHash|" + Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "|SHA1";
            
            ExecuteNonQuery(@"
                INSERT INTO AspNetUsers (
                    Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp,
                    PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount
                ) VALUES (
                    @Id, @Email, @Email, 1, @PasswordHash, @SecurityStamp,
                    0, 0, 1, 0
                )",
                new { 
                    Id = _legacyUserId, 
                    Email = _legacyEmail, 
                    PasswordHash = legacyHash, 
                    SecurityStamp = Guid.NewGuid().ToString() 
                });
        }

        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }

        [Fact]
        public async Task UserExistsAsync_WithExistingUser_ReturnsTrue()
        {
            // Act
            bool result = await _repository.UserExistsAsync(_existingEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithNonExistingUser_ReturnsFalse()
        {
            // Act
            bool result = await _repository.UserExistsAsync("nonexisting@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserIdByEmailAsync_WithExistingUser_ReturnsUserId()
        {
            // Act
            string result = await _repository.GetUserIdByEmailAsync(_existingEmail);

            // Assert
            Assert.Equal(_existingUserId, result);
        }

        [Fact]
        public async Task GetUserIdByEmailAsync_WithNonExistingUser_ReturnsNull()
        {
            // Act
            string result = await _repository.GetUserIdByEmailAsync("nonexisting@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateUserAsync_WithValidCredentials_ReturnsTrue()
        {
            // Act
            bool result = await _repository.ValidateUserAsync(_existingEmail, _validPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateUserAsync_WithInvalidCredentials_ReturnsFalse()
        {
            // Act
            bool result = await _repository.ValidateUserAsync(_existingEmail, "WrongPassword");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateUserAsync_WithNonExistingUser_ReturnsFalse()
        {
            // Act
            bool result = await _repository.ValidateUserAsync("nonexistent@example.com", _validPassword);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task CreateUserAsync_Success_ReturnsTrue()
        {
            // Arrange
            const string email = "new@example.com";
                
            // Act
            bool result = await _repository.CreateUserAsync(email, _validPassword);
            
            // Assert
            Assert.True(result);
            
            // Verify the user was actually created in the database
            string sql = "SELECT COUNT(1) FROM AspNetUsers WHERE Email = @Email";
            int count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Email = email });
            Assert.Equal(1, count);
        }
        
        [Fact]
        public async Task CreateUserAsync_DuplicateEmail_ReturnsFalse()
        {
            // Act
            bool result = await _repository.CreateUserAsync(_existingEmail, _validPassword);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task UpdatePasswordHashAsync_Success_ReturnsTrue()
        {
            // Arrange
            const string newHashedPassword = "newHashedPassword";
                
            // Act
            bool result = await _repository.UpdatePasswordHashAsync(_existingUserId, newHashedPassword);
            
            // Assert
            Assert.True(result);
            
            // Verify the password hash was actually updated in the database
            string sql = "SELECT PasswordHash FROM AspNetUsers WHERE Id = @Id";
            string updatedHash = await _dbConnection.QuerySingleOrDefaultAsync<string>(sql, new { Id = _existingUserId });
            Assert.Equal(newHashedPassword, updatedHash);
        }
        
        [Fact]
        public async Task UpdatePasswordHashAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            const string nonExistentUserId = "nonexistent";
            const string newHashedPassword = "newHashedPassword";
                
            // Act
            bool result = await _repository.UpdatePasswordHashAsync(nonExistentUserId, newHashedPassword);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task ValidateUserAsync_WithLegacyHashNeedingRehash_UpdatesHashAndReturnsTrue()
        {
            // Arrange - Create a special password hasher that will always return SuccessRehashNeeded for legacy hashes
            var specialHasher = new TestLegacyPasswordHasher();
            var specialRepository = new UserRepository(_dbConnection, specialHasher);

            // Act
            bool result = await specialRepository.ValidateUserAsync(_legacyEmail, _validPassword);

            // Assert
            Assert.True(result);
            
            // Verify the password hash was updated by checking if it's different now
            string sql = "SELECT PasswordHash FROM AspNetUsers WHERE Email = @Email";
            string updatedHash = await _dbConnection.QuerySingleOrDefaultAsync<string>(sql, new { Email = _legacyEmail });
            
            // The hash should be updated and no longer in legacy format
            Assert.False(updatedHash.Contains("|"));
        }
        
        // Special test password hasher that simulates legacy hash verification
        private class TestLegacyPasswordHasher : LegacyCompatiblePasswordHasher<IdentityUser>
        {
            public override PasswordVerificationResult VerifyHashedPassword(
                IdentityUser user, string hashedPassword, string providedPassword)
            {
                // For testing, always return SuccessRehashNeeded if the hash contains a pipe character
                // (which is our marker for legacy hash format)
                if (hashedPassword != null && hashedPassword.Contains("|"))
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                
                return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
            }
        }
    }
}
