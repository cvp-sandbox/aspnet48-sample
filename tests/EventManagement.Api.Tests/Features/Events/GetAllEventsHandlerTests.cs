using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetAllEventsHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetAllEventsHandler> _logger;
    private readonly GetAllEventsHandler _handler;

    public GetAllEventsHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _logger = Substitute.For<ILogger<GetAllEventsHandler>>();
        _handler = new GetAllEventsHandler(_eventRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event
            {
                EventId = 1,
                Name = "Test Event 1",
                Description = "Description 1",
                EventDate = DateTime.Now.AddDays(1),
                Location = "Location 1",
                MaxAttendees = 10,
                CreatedBy = "user1",
                CreatedDate = DateTime.Now
            },
            new Event
            {
                EventId = 2,
                Name = "Test Event 2",
                Description = "Description 2",
                EventDate = DateTime.Now.AddDays(2),
                Location = "Location 2",
                MaxAttendees = 20,
                CreatedBy = "user2",
                CreatedDate = DateTime.Now
            }
        };

        _eventRepository.GetAllEventsAsync().Returns(events);

        // Act
        var response = await _handler.HandleAsync(new GetAllEventsRequest());

        // Assert
        response.Events.Should().BeEquivalentTo(events);
        await _eventRepository.Received(1).GetAllEventsAsync();
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldRethrowException()
    {
        // Arrange
        _eventRepository.GetAllEventsAsync().Returns(Task.FromException<IEnumerable<Event>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(new GetAllEventsRequest()));
    }
}