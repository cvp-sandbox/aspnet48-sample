import { Row, Col, Alert, Spinner } from 'react-bootstrap';
import { useFeaturedEvents } from '../../hooks/useEvents';
import EventCard from './EventCard';
import { Event } from '../../types/api';

/**
 * Component that displays featured events
 */
const FeaturedEvents = () => {
  // Fetch featured events
  const { data, isLoading, error } = useFeaturedEvents();
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
        Error loading featured events: {(error as Error).message}
      </Alert>
    );
  }
  
  if (events.length === 0) {
    return (
      <Alert variant="info">
        There are no featured events at this time.
      </Alert>
    );
  }
  
  return (
    <div>
      <h2 className="mb-4">Featured Events</h2>
      <Row xs={1} md={2} lg={3} className="g-4">
        {events.map((event) => (
          <Col key={event.eventId}>
            <EventCard event={event} />
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default FeaturedEvents;
