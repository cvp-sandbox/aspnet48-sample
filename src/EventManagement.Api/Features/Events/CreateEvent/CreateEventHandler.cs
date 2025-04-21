using EventManagement.Api.Models;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.CreateEvent;

public class CreateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<CreateEventHandler> _logger;

    public CreateEventHandler(
        IEventRepository eventRepository,
        ILogger<CreateEventHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<CreateEventResponse> HandleAsync(CreateEventRequest request, string userId)
    {
        _logger.LogInformation("Creating new event: {EventName}", request.Name);
        
        try
        {
            var eventEntity = new Event
            {
                Name = request.Name,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                MaxAttendees = request.MaxAttendees,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };
            
            int eventId = await _eventRepository.CreateEventAsync(eventEntity);
            
            return new CreateEventResponse(eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event: {EventName}", request.Name);
            throw;
        }
    }
}
