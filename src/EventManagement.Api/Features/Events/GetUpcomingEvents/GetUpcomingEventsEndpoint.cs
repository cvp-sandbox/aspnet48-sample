using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetUpcomingEvents;

public static class GetUpcomingEventsEndpoint
{
    public static WebApplication MapGetUpcomingEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/upcoming", async (
            HttpContext context,
            [FromServices] GetUpcomingEventsHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name;
            
            if (string.IsNullOrEmpty(username))
            {
                return Results.BadRequest("Username is required");
            }
            
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
