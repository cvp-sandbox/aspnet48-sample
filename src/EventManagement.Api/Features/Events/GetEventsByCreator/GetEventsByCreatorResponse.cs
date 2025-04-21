using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetEventsByCreator;

public record GetEventsByCreatorResponse(IEnumerable<Event> Events);
