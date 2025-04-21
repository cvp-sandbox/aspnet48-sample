using EventManagement.Api.Models;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.RegisterForEvent;

public class RegisterForEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<RegisterForEventHandler> _logger;

    public RegisterForEventHandler(
        IEventRepository eventRepository,
        IRegistrationRepository registrationRepository,
        ILogger<RegisterForEventHandler> logger)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<RegisterForEventResponse> HandleAsync(RegisterForEventRequest request, string userId)
    {
        _logger.LogInformation("Registering user {UserId} for event {EventId}", userId, request.EventId);
        
        try
        {
            // Check if event exists
            var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId);
            if (eventEntity == null)
            {
                return new RegisterForEventResponse(false, "Event not found");
            }
            
            // Check if already registered
            var existingRegistration = await _registrationRepository.GetRegistrationAsync(request.EventId, userId);
            if (existingRegistration != null)
            {
                return new RegisterForEventResponse(false, "You are already registered for this event");
            }
            
            // Check if event is full
            int registrationCount = await _eventRepository.GetRegistrationCountAsync(request.EventId);
            if (eventEntity.MaxAttendees > 0 && registrationCount >= eventEntity.MaxAttendees)
            {
                return new RegisterForEventResponse(false, "This event is already at full capacity");
            }
            
            // Create registration
            var registration = new Registration
            {
                EventId = request.EventId,
                UserId = userId,
                RegistrationDate = DateTime.Now
            };
            
            int registrationId = await _registrationRepository.CreateRegistrationAsync(registration);
            
            return new RegisterForEventResponse(true, "You have successfully registered for this event", registrationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {UserId} for event {EventId}", userId, request.EventId);
            throw;
        }
    }
}
