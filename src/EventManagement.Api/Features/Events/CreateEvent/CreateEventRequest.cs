namespace EventManagement.Api.Features.Events.CreateEvent;

public record CreateEventRequest(
    string Name,
    string? Description,
    DateTime EventDate,
    string? Location,
    int MaxAttendees);
