using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetUpcomingEvents;

public record RegistrationWithEvent
{
    public int RegistrationId { get; init; }
    public int EventId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public DateTime RegistrationDate { get; init; }
    public string UserName { get; init; } = string.Empty;
    public Event Event { get; set; } = new Event();
}

public record GetUpcomingEventsResponse(IEnumerable<RegistrationWithEvent> Registrations);
