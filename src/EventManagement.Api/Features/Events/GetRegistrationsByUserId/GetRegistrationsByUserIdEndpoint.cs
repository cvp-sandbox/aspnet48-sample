using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetRegistrationsByUserId;

public static class GetRegistrationsByUserIdEndpoint
{
    public static WebApplication MapGetRegistrationsByUserIdEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/my-registrations", async (
            HttpContext context,
            [FromServices] GetRegistrationsByUserIdHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            // Check if user is authenticated
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }
            
            var request = new GetRegistrationsByUserIdRequest(username);
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetRegistrationsByUserId")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
