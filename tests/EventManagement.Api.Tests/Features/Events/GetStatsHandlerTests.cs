using EventManagement.Api.Features.Events.GetStats;
using EventManagement.Api.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetStatsHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetStatsHandler> _logger;
    private readonly GetStatsHandler _handler;

    public GetStatsHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _logger = Substitute.For<ILogger<GetStatsHandler>>();
        _handler = new GetStatsHandler(_eventRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnStats()
    {
        // Arrange
        _eventRepository.GetActiveEventsCountAsync().Returns(10);
        _eventRepository.GetThisWeekEventsCountAsync().Returns(5);
        _eventRepository.GetRegisteredUsersForFutureEventsCountAsync().Returns(20);

        // Act
        var result = await _handler.HandleAsync(new GetStatsRequest());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Stats.Count());
        
        var statsList = result.Stats.ToList();
        Assert.Equal("ActiveEvents", statsList[0].StatLabel);
        Assert.Equal(10, statsList[0].StatValue);
        
        Assert.Equal("ThisWeeksEvents", statsList[1].StatLabel);
        Assert.Equal(5, statsList[1].StatValue);
        
        Assert.Equal("RegisteredUsers", statsList[2].StatLabel);
        Assert.Equal(20, statsList[2].StatValue);
        
        await _eventRepository.Received(1).GetActiveEventsCountAsync();
        await _eventRepository.Received(1).GetThisWeekEventsCountAsync();
        await _eventRepository.Received(1).GetRegisteredUsersForFutureEventsCountAsync();
    }
}
