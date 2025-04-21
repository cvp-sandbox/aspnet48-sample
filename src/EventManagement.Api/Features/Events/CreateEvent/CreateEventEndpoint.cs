using Microsoft.AspNetCore.Mvc;
using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.CreateEvent;

public static class CreateEventEndpoint
{
    public static WebApplication MapCreateEventEndpoint(this WebApplication app)
    {
        app.MapPost("/api/events", async (
            [FromBody] CreateEventRequest request,
            HttpContext context,
            [FromServices] CreateEventHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            // Check if user has required roles
            bool isAdmin = user.IsInRole(Roles.Admin.Name);
            bool isOrganizer = user.IsInRole(Roles.Organizer.Name);
            
            if (!isAdmin && !isOrganizer)
            {
                return Results.Forbid();
            }
            
            var response = await handler.HandleAsync(request, username);
            
            return Results.Created($"/api/events/{response.EventId}", response);
        })
        .WithName("CreateEvent")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
