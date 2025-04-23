using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Api.Features.Users.Login;
using EventManagement.Api.Repositories;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Users
{
    public class LoginHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _handler = new LoginHandler(_userRepository, _roleRepository);
        }

        [Fact]
        public async Task HandleAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _userRepository.ValidateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(true));
            
            _userRepository.GetUserIdByEmailAsync(request.Email)
                .Returns(Task.FromResult("user123"));
            
            _roleRepository.GetUserRolesAsync("user123")
                .Returns(Task.FromResult<IEnumerable<string>>(new[] { "User" }));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("user123", response.UserId);
            Assert.Equal("test@example.com", response.Email);
            Assert.Contains("User", response.Roles);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public async Task HandleAsync_WithInvalidCredentials_ReturnsFailureResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _userRepository.ValidateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(false));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Invalid email or password.", response.ErrorMessage);
        }

        [Fact]
        public async Task HandleAsync_WithUserNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _userRepository.ValidateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(true));
            
            _userRepository.GetUserIdByEmailAsync(request.Email)
                .Returns(Task.FromResult<string>(null));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("User not found.", response.ErrorMessage);
        }

        [Fact]
        public async Task HandleAsync_WithReturnUrl_PreservesReturnUrl()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ReturnUrl = "/events/123"
            };

            _userRepository.ValidateUserAsync(request.Email, request.Password)
                .Returns(Task.FromResult(true));
            
            _userRepository.GetUserIdByEmailAsync(request.Email)
                .Returns(Task.FromResult("user123"));
            
            _roleRepository.GetUserRolesAsync("user123")
                .Returns(Task.FromResult<IEnumerable<string>>(new[] { "User" }));

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("/events/123", response.ReturnUrl);
        }
    }
}