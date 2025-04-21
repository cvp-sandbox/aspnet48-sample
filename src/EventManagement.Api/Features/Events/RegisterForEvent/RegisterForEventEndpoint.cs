using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.RegisterForEvent;

public static class RegisterForEventEndpoint
{
    public static WebApplication MapRegisterForEventEndpoint(this WebApplication app)
    {
        app.MapPost("/api/events/{id}/register", async (
            int id,
            HttpContext context,
            [FromServices] RegisterForEventHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            // Check if user is authenticated
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }
            
            var request = new RegisterForEventRequest(id);
            var response = await handler.HandleAsync(request, username);
            
            if (!response.Success)
            {
                return Results.BadRequest(new { message = response.Message });
            }
            
            return Results.Ok(response);
        })
        .WithName("RegisterForEvent")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
