using EventManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public static class GetAllEventsEndpoint
{
    public static WebApplication MapGetAllEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events", async (
        HttpContext context,
        [FromServices] GetAllEventsHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name;
            var isAdmin = user.IsInRole(Roles.Admin.Name); 

            var request = new GetAllEventsRequest();
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetAllEvents")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}