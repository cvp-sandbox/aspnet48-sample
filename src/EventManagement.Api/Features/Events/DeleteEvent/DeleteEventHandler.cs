using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.DeleteEvent;

public class DeleteEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<DeleteEventHandler> _logger;

    public DeleteEventHandler(
        IEventRepository eventRepository,
        ILogger<DeleteEventHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<DeleteEventResponse> HandleAsync(DeleteEventRequest request, string userId, bool isAdmin)
    {
        _logger.LogInformation("Deleting event with ID {EventId}", request.EventId);
        
        try
        {
            // Get the original event to check ownership
            var originalEvent = await _eventRepository.GetEventByIdAsync(request.EventId);
            
            if (originalEvent == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", request.EventId);
                return new DeleteEventResponse(false);
            }
            
            // Check if user is authorized to delete this event
            if (originalEvent.CreatedBy != userId && !isAdmin)
            {
                _logger.LogWarning("User {UserId} not authorized to delete event {EventId}", userId, request.EventId);
                return new DeleteEventResponse(false);
            }
            
            // Delete the event
            bool success = await _eventRepository.DeleteEventAsync(request.EventId);
            
            return new DeleteEventResponse(success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event with ID {EventId}", request.EventId);
            throw;
        }
    }
}
