using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Api.Features.Users.Register;
using EventManagement.Api.Repositories;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Users
{
    public class RegisterHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly RegisterHandler _handler;

        public RegisterHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _handler = new RegisterHandler(_userRepository, _roleRepository);
        }

        [Fact]
        public async Task HandleAsync_WithValidData_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userRepository.UserExistsAsync(request.Email)
                .Returns(Task.FromResult(false));
            
            _userRepository.CreateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(true));
            
            _userRepository.GetUserIdByEmailAsync(request.Email)
                .Returns(Task.FromResult("user123"));
            
            _roleRepository.AddUserToRoleAsync("user123", "User")
                .Returns(Task.FromResult(true));
            
            _roleRepository.GetUserRolesAsync("user123")
                .Returns(Task.FromResult<IEnumerable<string>>(new[] { "User" }));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("user123", response.UserId);
            Assert.Equal("test@example.com", response.Email);
            Assert.Contains("User", response.Roles);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task HandleAsync_WithExistingEmail_ReturnsFailureResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userRepository.UserExistsAsync(request.Email)
                .Returns(Task.FromResult(true));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Contains("User with this email already exists.", response.Errors);
        }

        [Fact]
        public async Task HandleAsync_WithPasswordMismatch_ReturnsFailureResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword!"
            };

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Contains("The password and confirmation password do not match.", response.Errors);
        }
        
        [Fact]
        public async Task HandleAsync_WithFailedUserCreation_ReturnsFailureResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userRepository.UserExistsAsync(request.Email)
                .Returns(Task.FromResult(false));
            
            _userRepository.CreateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(false));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Contains("Failed to create user.", response.Errors);
        }
        
        [Fact]
        public async Task HandleAsync_WithFailedUserIdRetrieval_ReturnsFailureResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _userRepository.UserExistsAsync(request.Email)
                .Returns(Task.FromResult(false));
            
            _userRepository.CreateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(true));
            
            _userRepository.GetUserIdByEmailAsync(request.Email)
                .Returns(Task.FromResult<string>(null));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Contains("Failed to retrieve user ID.", response.Errors);
        }
    }
}