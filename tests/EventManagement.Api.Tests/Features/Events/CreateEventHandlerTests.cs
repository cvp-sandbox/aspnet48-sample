using EventManagement.Api.Features.Events.CreateEvent;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class CreateEventHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<CreateEventHandler> _logger;
    private readonly CreateEventHandler _handler;

    public CreateEventHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _logger = Substitute.For<ILogger<CreateEventHandler>>();
        _handler = new CreateEventHandler(_eventRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateEvent_AndReturnEventId()
    {
        // Arrange
        var userId = "user1";
        var request = new CreateEventRequest(
            "Test Event",
            "Test Description",
            DateTime.Now.AddDays(1),
            "Test Location",
            10);

        var expectedEventId = 1;
        _eventRepository.CreateEventAsync(Arg.Any<Event>()).Returns(expectedEventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.EventId.Should().Be(expectedEventId);
        
        // Verify the event was created with correct properties
        await _eventRepository.Received(1).CreateEventAsync(Arg.Is<Event>(e => 
            e.Name == request.Name &&
            e.Description == request.Description &&
            e.EventDate == request.EventDate &&
            e.Location == request.Location &&
            e.MaxAttendees == request.MaxAttendees &&
            e.CreatedBy == userId));
    }

    [Fact]
    public async Task HandleAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var userId = "user1";
        var request = new CreateEventRequest(
            "Test Event",
            "Test Description",
            DateTime.Now.AddDays(1),
            "Test Location",
            10);

        var expectedException = new Exception("Test exception");
        _eventRepository.When(x => x.CreateEventAsync(Arg.Any<Event>()))
            .Do(x => { throw expectedException; });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(request, userId));
        
        // Verify error was logged
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            expectedException,
            Arg.Any<Func<object, Exception, string>>());
    }
}
