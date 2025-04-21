using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetRegistrationsByUserId;

public class GetRegistrationsByUserIdHandler
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<GetRegistrationsByUserIdHandler> _logger;

    public GetRegistrationsByUserIdHandler(
        IRegistrationRepository registrationRepository,
        ILogger<GetRegistrationsByUserIdHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<GetRegistrationsByUserIdResponse> HandleAsync(GetRegistrationsByUserIdRequest request)
    {
        _logger.LogInformation("Retrieving registrations for user {UserId}", request.UserId);
        
        try
        {
            var registrations = await _registrationRepository.GetRegistrationsByUserIdAsync(request.UserId);
            return new GetRegistrationsByUserIdResponse(registrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving registrations for user {UserId}", request.UserId);
            throw;
        }
    }
}
