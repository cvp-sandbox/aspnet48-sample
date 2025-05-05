using EventManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetFeaturedEvents;

public static class GetFeaturedEventsEndpoint
{
    public static WebApplication MapGetFeaturedEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/featured", async (
        [FromServices] GetFeaturedEventsHandler handler) =>
        {
            // No authentication required for featured events
            var request = new GetFeaturedEventsRequest();
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetFeaturedEvents")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
