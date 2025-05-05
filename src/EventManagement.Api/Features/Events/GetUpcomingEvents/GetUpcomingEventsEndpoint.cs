using EventManagement.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetUpcomingEvents;

public static class GetUpcomingEventsEndpoint
{
    public static WebApplication MapGetUpcomingEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/upcoming", async (
            HttpContext context,
            [FromServices] GetUpcomingEventsHandler handler,
            [FromServices] IEventRepository eventRepository) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name;
            
            // If user is not authenticated, return all upcoming events
            if (string.IsNullOrEmpty(username))
            {
                // Get all events in the next 7 days
                var upcomingEvents = await eventRepository.GetAllEventsAsync();
                var filteredEvents = upcomingEvents
                    .Where(e => e.EventDate > DateTime.Now && e.EventDate <= DateTime.Now.AddDays(7))
                    .ToList();
                
                return Results.Ok(new { events = filteredEvents });
            }
            
            // If user is authenticated, return their upcoming events
            var request = new GetUpcomingEventsRequest(username);
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetUpcomingEvents")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
