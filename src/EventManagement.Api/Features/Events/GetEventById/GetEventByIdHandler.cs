using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetEventById;

public class GetEventByIdHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<GetEventByIdHandler> _logger;

    public GetEventByIdHandler(
        IEventRepository eventRepository,
        IRegistrationRepository registrationRepository,
        ILogger<GetEventByIdHandler> logger)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<GetEventByIdResponse> HandleAsync(GetEventByIdRequest request, string userId)
    {
        _logger.LogInformation("Retrieving event with ID {EventId}", request.EventId);
        
        try
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId);
            
            if (eventEntity == null)
            {
                return new GetEventByIdResponse(null, false, false, 0);
            }
            
            // Check if user is registered
            var registration = await _registrationRepository.GetRegistrationAsync(request.EventId, userId);
            bool isRegistered = registration != null;
            
            // Check if user is the creator
            bool isCreator = eventEntity.CreatedBy == userId;
            
            // Get registration count
            int registrationCount = await _eventRepository.GetRegistrationCountAsync(request.EventId);
            
            return new GetEventByIdResponse(eventEntity, isRegistered, isCreator, registrationCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event with ID {EventId}", request.EventId);
            throw;
        }
    }
}
