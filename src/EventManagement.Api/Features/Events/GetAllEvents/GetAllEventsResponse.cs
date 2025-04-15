using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public record GetAllEventsResponse(IEnumerable<Event> Events);