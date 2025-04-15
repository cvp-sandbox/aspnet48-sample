using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public class GetAllEventsHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetAllEventsHandler> _logger;

    public GetAllEventsHandler(IEventRepository eventRepository, ILogger<GetAllEventsHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<GetAllEventsResponse> HandleAsync(GetAllEventsRequest request)
    {
        _logger.LogInformation("Retrieving all events");
        
        try
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return new GetAllEventsResponse(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events");
            throw;
        }
    }
}