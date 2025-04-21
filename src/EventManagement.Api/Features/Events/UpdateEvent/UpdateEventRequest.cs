namespace EventManagement.Api.Features.Events.UpdateEvent;

public record UpdateEventRequest(
    int EventId,
    string Name,
    string? Description,
    DateTime EventDate,
    string? Location,
    int MaxAttendees);
