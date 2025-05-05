import { Row, Col, Alert, Spinner, Card, Button, Badge } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useMyRegistrations, useCancelRegistration } from '../../hooks/useEvents';
import { Event } from '../../types/api';

/**
 * Component that displays events the current user is registered for
 */
const MyRegistrations = () => {
  // Fetch events the user is registered for
  const { data, isLoading, error, refetch } = useMyRegistrations();
  const events = data?.events as Event[] || [];
  
  // Cancel registration mutation
  const { mutate: cancelRegistration, isPending: isCancelling } = useCancelRegistration();
  
  // Format the date
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  };
  
  // Handle cancellation
  const handleCancel = (eventId: number) => {
    cancelRegistration(eventId, {
      onSuccess: () => {
        // Refetch registrations to update UI
        refetch();
      },
    });
  };
  
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
        Error loading your registrations: {(error as Error).message}
      </Alert>
    );
  }
  
  return (
    <div>
      <h1 className="mb-4">My Registrations</h1>
      
      {events.length === 0 ? (
        <Alert variant="info">
          You haven't registered for any events yet. <Link to="/events">Browse events</Link> to find something interesting!
        </Alert>
      ) : (
        <div>
          <Row xs={1} className="g-4">
            {events.map((event) => {
              const isUpcoming = new Date(event.date) > new Date();
              
              return (
                <Col key={event.eventId}>
                  <Card>
                    <Card.Body>
                      <div className="d-flex justify-content-between align-items-start">
                        <div>
                          <Card.Title>{event.title}</Card.Title>
                          <Card.Subtitle className="mb-2 text-muted">
                            {formatDate(event.date)}
                          </Card.Subtitle>
                          
                          <div className="mb-2">
                            {event.isFeatured && (
                              <Badge bg="warning" className="me-1">Featured</Badge>
                            )}
                            {isUpcoming ? (
                              <Badge bg="success" className="me-1">Upcoming</Badge>
                            ) : (
                              <Badge bg="secondary" className="me-1">Past</Badge>
                            )}
                            {event.location && (
                              <span className="text-muted">
                                <i className="bi bi-geo-alt"></i> {event.location}
                              </span>
                            )}
                          </div>
                          
                          <Card.Text>
                            {event.description && event.description.length > 150
                              ? `${event.description.substring(0, 150)}...`
                              : event.description || 'No description available'}
                          </Card.Text>
                        </div>
                        
                        <div className="d-flex flex-column">
                          <Button
                            as={Link as any}
                            to={`/events/${event.eventId}`}
                            variant="outline-primary"
                            className="mb-2"
                          >
                            View Details
                          </Button>
                          
                          {isUpcoming && (
                            <Button
                              variant="outline-danger"
                              onClick={() => handleCancel(event.eventId)}
                              disabled={isCancelling}
                            >
                              Cancel Registration
                            </Button>
                          )}
                        </div>
                      </div>
                    </Card.Body>
                  </Card>
                </Col>
              );
            })}
          </Row>
        </div>
      )}
    </div>
  );
};

export default MyRegistrations;
