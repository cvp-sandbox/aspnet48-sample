using EventManagement.Api.Models;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.UpdateEvent;

public class UpdateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<UpdateEventHandler> _logger;

    public UpdateEventHandler(
        IEventRepository eventRepository,
        ILogger<UpdateEventHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<UpdateEventResponse> HandleAsync(UpdateEventRequest request, string userId, bool isAdmin)
    {
        _logger.LogInformation("Updating event with ID {EventId}", request.EventId);
        
        try
        {
            // Get the original event to check ownership
            var originalEvent = await _eventRepository.GetEventByIdAsync(request.EventId);
            
            if (originalEvent == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", request.EventId);
                return new UpdateEventResponse(false);
            }
            
            // Check if user is authorized to update this event
            if (originalEvent.CreatedBy != userId && !isAdmin)
            {
                _logger.LogWarning("User {UserId} not authorized to update event {EventId}", userId, request.EventId);
                return new UpdateEventResponse(false);
            }
            
            // Update the event
            var eventEntity = new Event
            {
                EventId = request.EventId,
                Name = request.Name,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                MaxAttendees = request.MaxAttendees,
                // Preserve original values
                CreatedBy = originalEvent.CreatedBy,
                CreatedDate = originalEvent.CreatedDate
            };
            
            bool success = await _eventRepository.UpdateEventAsync(eventEntity);
            
            return new UpdateEventResponse(success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event with ID {EventId}", request.EventId);
            throw;
        }
    }
}
