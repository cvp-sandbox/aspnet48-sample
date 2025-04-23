using System.Threading.Tasks;
using EventManagement.Api.Features.Users.LogOff;
using Xunit;

namespace EventManagement.Api.Tests.Features.Users
{
    public class LogOffHandlerTests
    {
        private readonly LogOffHandler _handler;

        public LogOffHandlerTests()
        {
            _handler = new LogOffHandler();
        }

        [Fact]
        public async Task HandleAsync_AlwaysReturnsSuccessResponse()
        {
            // Arrange
            var request = new LogOffRequest();

            // Act
            var response = await _handler.HandleAsync(request);

            // Assert
            Assert.True(response.Success);
        }
    }
}