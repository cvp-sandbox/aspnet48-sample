using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.CancelRegistration;

public static class CancelRegistrationEndpoint
{
    public static WebApplication MapCancelRegistrationEndpoint(this WebApplication app)
    {
        app.MapPost("/api/events/{id}/cancel-registration", async (
            int id,
            HttpContext context,
            [FromServices] CancelRegistrationHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            // Check if user is authenticated
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }
            
            var request = new CancelRegistrationRequest(id);
            var response = await handler.HandleAsync(request, username);
            
            if (!response.Success)
            {
                return Results.BadRequest(new { message = response.Message });
            }
            
            return Results.Ok(response);
        })
        .WithName("CancelRegistration")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
