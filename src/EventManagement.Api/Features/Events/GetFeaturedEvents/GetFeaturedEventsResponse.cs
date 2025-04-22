using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetFeaturedEvents;

public record GetFeaturedEventsResponse(IEnumerable<Event> Events);
