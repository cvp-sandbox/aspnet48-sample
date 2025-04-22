using EventManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetFeaturedEvents;

public static class GetFeaturedEventsEndpoint
{
    public static WebApplication MapGetFeaturedEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/featured", async (
        HttpContext context,
        [FromServices] GetFeaturedEventsHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name;
            var isAdmin = user.IsInRole(Roles.Admin.Name); 

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
