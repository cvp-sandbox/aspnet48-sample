using Microsoft.AspNetCore.Mvc;
using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.UpdateEvent;

public static class UpdateEventEndpoint
{
    public static WebApplication MapUpdateEventEndpoint(this WebApplication app)
    {
        app.MapPut("/api/events/{id}", async (
            int id,
            [FromBody] UpdateEventRequest request,
            HttpContext context,
            [FromServices] UpdateEventHandler handler) =>
        {
            // Ensure the ID in the route matches the ID in the request
            if (id != request.EventId)
            {
                return Results.BadRequest("Event ID mismatch between route and request body");
            }
            
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
            
            var response = await handler.HandleAsync(request, username, isAdmin);
            
            if (!response.Success)
            {
                return Results.NotFound();
            }
            
            return Results.NoContent();
        })
        .WithName("UpdateEvent")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
