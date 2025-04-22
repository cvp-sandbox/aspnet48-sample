using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetFeaturedEvents;

public class GetFeaturedEventsHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetFeaturedEventsHandler> _logger;

    public GetFeaturedEventsHandler(IEventRepository eventRepository, ILogger<GetFeaturedEventsHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<GetFeaturedEventsResponse> HandleAsync(GetFeaturedEventsRequest request)
    {
        _logger.LogInformation("Retrieving featured events");
        
        try
        {
            var events = await _eventRepository.GetFeaturedEventsAsync();
            return new GetFeaturedEventsResponse(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving featured events");
            throw;
        }
    }
}
