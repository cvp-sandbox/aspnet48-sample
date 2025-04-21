using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetEventById;

public static class GetEventByIdEndpoint
{
    public static WebApplication MapGetEventByIdEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/{id}", async (
            int id,
            HttpContext context,
            [FromServices] GetEventByIdHandler handler) =>
        {
            // Access the authenticated user
            var user = context.User;
            var username = user.Identity?.Name ?? string.Empty;
            
            var request = new GetEventByIdRequest(id);
            var response = await handler.HandleAsync(request, username);
            
            if (response.Event == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(response);
        })
        .WithName("GetEventById")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
