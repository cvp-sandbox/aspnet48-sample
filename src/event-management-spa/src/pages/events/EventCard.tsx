import { Card, Button, Badge } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { Event } from '../../types/api';

interface EventCardProps {
  event: Event;
}

/**
 * Component that displays an event card
 * @param event - The event to display
 */
const EventCard = ({ event }: EventCardProps) => {
  // Format the date
  const formatDate = (dateString: string | null | undefined) => {
    if (!dateString) {
      return 'Date not available';
    }
    
    try {
      // Handle both date formats (backend uses EventDate, frontend expects date)
      const date = new Date(dateString);
      // Check if date is valid
      if (isNaN(date.getTime())) {
        console.warn('Invalid date format received:', dateString);
        return 'Invalid date';
      }
      
      return new Intl.DateTimeFormat('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(date);
    } catch (error) {
      console.error('Error formatting date:', error);
      return 'Date error';
    }
  };

  return (
    <Card className="h-100 shadow-sm event-card">
      <Card.Body>
        <Card.Title>{event.title || (event as any).name}</Card.Title>
        <Card.Subtitle className="mb-2 text-muted">
          {formatDate(event.date || (event as any).eventDate)}
        </Card.Subtitle>
        
        <div className="mb-2">
          {event.isFeatured && (
            <Badge bg="warning" className="me-1">Featured</Badge>
          )}
          {(event.date || (event as any).eventDate) && 
           !isNaN(new Date(event.date || (event as any).eventDate).getTime()) && 
           new Date(event.date || (event as any).eventDate) > new Date() && (
            <Badge bg="success" className="me-1">Upcoming</Badge>
          )}
          {event.maxAttendees && event.registeredAttendees >= event.maxAttendees && (
            <Badge bg="danger">Full</Badge>
          )}
        </div>
        
        <Card.Text>
          {event.description && event.description.length > 100
            ? `${event.description.substring(0, 100)}...`
            : event.description || 'No description available'}
        </Card.Text>
        
        <div className="d-flex justify-content-between align-items-center">
          <small className="text-muted">
            {event.registeredAttendees !== undefined ? event.registeredAttendees : (event as any).registrationCount || 0} / {event.maxAttendees || (event as any).maxAttendees || '∞'} attendees
          </small>
          <Button
            as={Link as any}
            to={`/events/${event.eventId}`}
            variant="outline-primary"
            size="sm"
          >
            View Details
          </Button>
        </div>
      </Card.Body>
    </Card>
  );
};

export default EventCard;
