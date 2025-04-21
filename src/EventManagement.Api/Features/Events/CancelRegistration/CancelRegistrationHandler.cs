using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.CancelRegistration;

public class CancelRegistrationHandler
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<CancelRegistrationHandler> _logger;

    public CancelRegistrationHandler(
        IRegistrationRepository registrationRepository,
        ILogger<CancelRegistrationHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<CancelRegistrationResponse> HandleAsync(CancelRegistrationRequest request, string userId)
    {
        _logger.LogInformation("Canceling registration for user {UserId} for event {EventId}", userId, request.EventId);
        
        try
        {
            // Check if registered
            var registration = await _registrationRepository.GetRegistrationAsync(request.EventId, userId);
            if (registration == null)
            {
                return new CancelRegistrationResponse(false, "You are not registered for this event");
            }
            
            // Cancel registration
            bool success = await _registrationRepository.CancelRegistrationAsync(request.EventId, userId);
            
            if (success)
            {
                return new CancelRegistrationResponse(true, "Your registration has been canceled");
            }
            else
            {
                return new CancelRegistrationResponse(false, "Failed to cancel registration");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling registration for user {UserId} for event {EventId}", userId, request.EventId);
            throw;
        }
    }
}
