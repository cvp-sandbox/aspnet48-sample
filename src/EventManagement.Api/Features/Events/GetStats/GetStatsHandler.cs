using EventManagement.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace EventManagement.Api.Features.Events.GetStats;

public class GetStatsHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetStatsHandler> _logger;

    public GetStatsHandler(IEventRepository eventRepository, ILogger<GetStatsHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<GetStatsResponse> HandleAsync(GetStatsRequest request)
    {
        _logger.LogInformation("Retrieving event statistics");
        
        try
        {
            var activeEventsCount = await _eventRepository.GetActiveEventsCountAsync();
            var thisWeekEventsCount = await _eventRepository.GetThisWeekEventsCountAsync();
            var registeredUsersCount = await _eventRepository.GetRegisteredUsersForFutureEventsCountAsync();
            
            var stats = new List<StatItem>
            {
                new StatItem("ActiveEvents", activeEventsCount),
                new StatItem("ThisWeeksEvents", thisWeekEventsCount),
                new StatItem("RegisteredUsers", registeredUsersCount)
            };
            
            return new GetStatsResponse(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event statistics");
            throw;
        }
    }
}
