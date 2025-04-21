using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetEventsByCreator;

public static class GetEventsByCreatorEndpoint
{
    public static WebApplication MapGetEventsByCreatorEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/my-events", async (
            HttpContext context,
            [FromServices] GetEventsByCreatorHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            // Check if user is authenticated
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }
            
            var request = new GetEventsByCreatorRequest(username);
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetEventsByCreator")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
