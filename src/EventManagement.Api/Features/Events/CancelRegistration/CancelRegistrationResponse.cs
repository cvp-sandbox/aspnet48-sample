namespace EventManagement.Api.Features.Events.CancelRegistration;

public record CancelRegistrationResponse(
    bool Success,
    string? Message);
