namespace EventManagement.Api.Features.Events.RegisterForEvent;

public record RegisterForEventResponse(
    bool Success,
    string? Message,
    int? RegistrationId = null);
