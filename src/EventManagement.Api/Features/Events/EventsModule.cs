using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IEventRepository, EventRepository>();
        
        // Register handlers
        services.AddScoped<GetAllEventsHandler>();
        
        return services;
    }
    
    public static WebApplication MapEventsEndpoints(this WebApplication app)
    {
        app.MapGetAllEventsEndpoint();
        
        return app;
    }
}