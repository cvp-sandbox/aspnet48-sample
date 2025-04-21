using Microsoft.AspNetCore.Mvc;
using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.DeleteEvent;

public static class DeleteEventEndpoint
{
    public static WebApplication MapDeleteEventEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/events/{id}", async (
            int id,
            HttpContext context,
            [FromServices] DeleteEventHandler handler) =>
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
            
            var request = new DeleteEventRequest(id);
            var response = await handler.HandleAsync(request, username, isAdmin);
            
            if (!response.Success)
            {
                return Results.NotFound();
            }
            
            return Results.NoContent();
        })
        .WithName("DeleteEvent")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
