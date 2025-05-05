import { useParams, useNavigate } from 'react-router-dom';
import { Container, Card, Alert, Spinner, Button } from 'react-bootstrap';
import EventForm from '../../components/events/EventForm';
import { useEventById, useUpdateEvent } from '../../hooks/useEvents';
import { UpdateEventRequest, CreateEventRequest } from '../../api/generated';
import { Event } from '../../types/api';

/**
 * Page component for editing an existing event
 */
const EditEvent = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const eventId = id ? parseInt(id, 10) : 0;
  
  // Fetch existing event data
  const { data, isLoading, error: fetchError } = useEventById(eventId);
  const event = data as Event;
  
  // Update mutation
  const { mutate, isPending, error: updateError } = useUpdateEvent();
  
  const handleSubmit = (data: UpdateEventRequest | CreateEventRequest) => {
    // Since we're in edit mode, we can safely cast to UpdateEventRequest
    const updateData = data as UpdateEventRequest;
    mutate(updateData, {
      onSuccess: () => {
        // Navigate back to the event details page after successful update
        navigate(`/events/${eventId}`);
      }
    });
  };
  
  if (isLoading) {
    return (
      <Container className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </Container>
    );
  }
  
  if (fetchError) {
    return (
      <Container className="my-5">
        <Alert variant="danger">
          Error loading event: {(fetchError as Error).message}
        </Alert>
      </Container>
    );
  }
  
  if (!event) {
    return (
      <Container className="my-5">
        <Alert variant="warning">
          Event not found. It may have been removed or you don't have permission to edit it.
        </Alert>
      </Container>
    );
  }
  
  return (
    <Container>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Edit Event</h1>
        <Button 
          variant="outline-secondary" 
          onClick={() => navigate(`/events/${eventId}`)}
        >
          Cancel
        </Button>
      </div>
      
      <Card>
        <Card.Body>
          <EventForm 
            event={event}
            isEdit={true}
            onSubmit={handleSubmit}
            isSubmitting={isPending}
            error={updateError as Error}
          />
        </Card.Body>
      </Card>
    </Container>
  );
};

export default EditEvent;
