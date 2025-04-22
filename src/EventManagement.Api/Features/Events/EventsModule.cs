using EventManagement.Api.Features.Events.CancelRegistration;
using EventManagement.Api.Features.Events.CreateEvent;
using EventManagement.Api.Features.Events.DeleteEvent;
using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Features.Events.GetEventById;
using EventManagement.Api.Features.Events.GetEventsByCreator;
using EventManagement.Api.Features.Events.GetFeaturedEvents;
using EventManagement.Api.Features.Events.GetRegistrationsByUserId;
using EventManagement.Api.Features.Events.GetStats;
using EventManagement.Api.Features.Events.GetUpcomingEvents;
using EventManagement.Api.Features.Events.RegisterForEvent;
using EventManagement.Api.Features.Events.UpdateEvent;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        
        // Register handlers
        services.AddScoped<GetAllEventsHandler>();
        services.AddScoped<GetEventByIdHandler>();
        services.AddScoped<CreateEventHandler>();
        services.AddScoped<UpdateEventHandler>();
        services.AddScoped<DeleteEventHandler>();
        services.AddScoped<RegisterForEventHandler>();
        services.AddScoped<CancelRegistrationHandler>();
        services.AddScoped<GetEventsByCreatorHandler>();
        services.AddScoped<GetRegistrationsByUserIdHandler>();
        services.AddScoped<GetFeaturedEventsHandler>();
        services.AddScoped<GetStatsHandler>();
        services.AddScoped<GetUpcomingEventsHandler>();
        
        return services;
    }
    
    public static WebApplication MapEventsEndpoints(this WebApplication app)
    {
        app.MapGetAllEventsEndpoint();
        app.MapGetEventByIdEndpoint();
        app.MapCreateEventEndpoint();
        app.MapUpdateEventEndpoint();
        app.MapDeleteEventEndpoint();
        app.MapRegisterForEventEndpoint();
        app.MapCancelRegistrationEndpoint();
        app.MapGetEventsByCreatorEndpoint();
        app.MapGetRegistrationsByUserIdEndpoint();
        app.MapGetFeaturedEventsEndpoint();
        app.MapGetStatsEndpoint();
        app.MapGetUpcomingEventsEndpoint();
        
        return app;
    }
}
