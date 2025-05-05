import { useNavigate } from 'react-router-dom';
import { Container, Card, Button } from 'react-bootstrap';
import EventForm from '../../components/events/EventForm';
import { useCreateEvent } from '../../hooks/useEvents';
import { CreateEventRequest } from '../../api/generated';

/**
 * Page component for creating a new event
 */
const CreateEvent = () => {
  const navigate = useNavigate();
  const { mutate, isPending, error } = useCreateEvent();
  
  const handleSubmit = (data: CreateEventRequest) => {
    mutate(data, {
      onSuccess: (response) => {
        // Navigate to the event details page after successful creation
        navigate(`/events/${response.eventId}`);
      }
    });
  };
  
  return (
    <Container>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Create New Event</h1>
        <Button 
          variant="outline-secondary" 
          onClick={() => navigate('/events')}
        >
          Cancel
        </Button>
      </div>
      
      <Card>
        <Card.Body>
          <EventForm 
            onSubmit={handleSubmit}
            isSubmitting={isPending}
            error={error as Error}
          />
        </Card.Body>
      </Card>
    </Container>
  );
};

export default CreateEvent;
