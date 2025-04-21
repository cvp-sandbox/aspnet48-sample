using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetEventsByCreator;

public class GetEventsByCreatorHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetEventsByCreatorHandler> _logger;

    public GetEventsByCreatorHandler(
        IEventRepository eventRepository,
        ILogger<GetEventsByCreatorHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<GetEventsByCreatorResponse> HandleAsync(GetEventsByCreatorRequest request)
    {
        _logger.LogInformation("Retrieving events created by user {UserId}", request.UserId);
        
        try
        {
            var events = await _eventRepository.GetEventsByCreatorAsync(request.UserId);
            return new GetEventsByCreatorResponse(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events created by user {UserId}", request.UserId);
            throw;
        }
    }
}
