using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetEventById;

public record GetEventByIdResponse(
    Event? Event, 
    bool IsRegistered, 
    bool IsCreator, 
    int RegistrationCount);
