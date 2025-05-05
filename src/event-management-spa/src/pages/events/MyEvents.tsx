import { Row, Col, Alert, Spinner, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useMyEvents } from '../../hooks/useEvents';
import EventCard from './EventCard';
import { Event } from '../../types/api';
import { FaPlus } from 'react-icons/fa';

/**
 * Component that displays events created by the current user
 */
const MyEvents = () => {
  // Fetch events created by the current user
  const { data, isLoading, error } = useMyEvents();
  const events = data?.events as Event[] || [];
  
  if (isLoading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }
  
  if (error) {
    return (
      <Alert variant="danger">
        Error loading your events: {(error as Error).message}
      </Alert>
    );
  }
  
  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>My Events</h1>
        <Button as={Link as any} to="/events/create" variant="primary">
          <FaPlus className="me-2" /> Create Event
        </Button>
      </div>
      
      {events.length === 0 ? (
        <Alert variant="info">
          You haven't created any events yet. Click the "Create Event" button to get started.
        </Alert>
      ) : (
        <Row xs={1} md={2} lg={3} className="g-4">
          {events.map((event) => (
            <Col key={event.eventId}>
              <EventCard event={event} />
            </Col>
          ))}
        </Row>
      )}
    </div>
  );
};

export default MyEvents;
