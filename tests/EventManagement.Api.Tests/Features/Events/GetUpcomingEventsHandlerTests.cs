using EventManagement.Api.Features.Events.GetUpcomingEvents;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetUpcomingEventsHandlerTests
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<GetUpcomingEventsHandler> _logger;
    private readonly GetUpcomingEventsHandler _handler;

    public GetUpcomingEventsHandlerTests()
    {
        _registrationRepository = Substitute.For<IRegistrationRepository>();
        _logger = Substitute.For<ILogger<GetUpcomingEventsHandler>>();
        _handler = new GetUpcomingEventsHandler(_registrationRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUpcomingEvents()
    {
        // Arrange
        var username = "testuser";
        var expectedRegistrations = new List<RegistrationWithEvent>
        {
            new RegistrationWithEvent
            {
                RegistrationId = 1,
                EventId = 1,
                UserId = "user1",
                RegistrationDate = DateTime.Now.AddDays(-1),
                UserName = username,
                Event = new Event
                {
                    EventId = 1,
                    Name = "Event 1",
                    EventDate = DateTime.Now.AddDays(2)
                }
            },
            new RegistrationWithEvent
            {
                RegistrationId = 2,
                EventId = 2,
                UserId = "user1",
                RegistrationDate = DateTime.Now.AddDays(-2),
                UserName = username,
                Event = new Event
                {
                    EventId = 2,
                    Name = "Event 2",
                    EventDate = DateTime.Now.AddDays(5)
                }
            }
        };

        _registrationRepository.GetUpcomingEventsByUsernameAsync(username).Returns(expectedRegistrations);

        // Act
        var result = await _handler.HandleAsync(new GetUpcomingEventsRequest(username));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRegistrations, result.Registrations);
        await _registrationRepository.Received(1).GetUpcomingEventsByUsernameAsync(username);
    }
}
