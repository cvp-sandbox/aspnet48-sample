using EventManagement.Api.Features.Events.GetFeaturedEvents;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetFeaturedEventsHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetFeaturedEventsHandler> _logger;
    private readonly GetFeaturedEventsHandler _handler;

    public GetFeaturedEventsHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _logger = Substitute.For<ILogger<GetFeaturedEventsHandler>>();
        _handler = new GetFeaturedEventsHandler(_eventRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFeaturedEvents()
    {
        // Arrange
        var expectedEvents = new List<Event>
        {
            new Event { EventId = 1, Name = "Event 1" },
            new Event { EventId = 2, Name = "Event 2" },
            new Event { EventId = 3, Name = "Event 3" }
        };

        _eventRepository.GetFeaturedEventsAsync().Returns(expectedEvents);

        // Act
        var result = await _handler.HandleAsync(new GetFeaturedEventsRequest());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEvents, result.Events);
        await _eventRepository.Received(1).GetFeaturedEventsAsync();
    }
}
