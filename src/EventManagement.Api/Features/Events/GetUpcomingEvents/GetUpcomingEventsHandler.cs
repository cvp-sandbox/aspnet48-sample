using EventManagement.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace EventManagement.Api.Features.Events.GetUpcomingEvents;

public class GetUpcomingEventsHandler
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<GetUpcomingEventsHandler> _logger;

    public GetUpcomingEventsHandler(IRegistrationRepository registrationRepository, ILogger<GetUpcomingEventsHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<GetUpcomingEventsResponse> HandleAsync(GetUpcomingEventsRequest request)
    {
        _logger.LogInformation("Retrieving upcoming events for user {Username}", request.Username);
        
        try
        {
            var registrations = await _registrationRepository.GetUpcomingEventsByUsernameAsync(request.Username);
            return new GetUpcomingEventsResponse(registrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming events for user {Username}", request.Username);
            throw;
        }
    }
}
