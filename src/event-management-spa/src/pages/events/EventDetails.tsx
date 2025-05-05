import { useParams, useNavigate } from 'react-router-dom';
import { Card, Button, Badge, Row, Col, Alert, Spinner, Modal } from 'react-bootstrap';
import { useEventById, useRegisterForEvent, useCancelRegistration, useDeleteEvent } from '../../hooks/useEvents';
import { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { Event } from '../../types/api';

/**
 * Component that displays detailed information about a specific event
 */
const EventDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, role } = useAuth();
  
  // State for delete confirmation modal
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  
  // Convert id to number
  const eventId = id ? parseInt(id, 10) : 0;
  
  // Fetch event details
  const { data, isLoading, error } = useEventById(eventId);
  
  // Debug logging
  console.log('API Response:', data);
  
  // Map API response to Event type
  const event = data?.event ? {
    eventId: data.event.eventId,
    title: data.event.name,
    description: data.event.description,
    date: data.event.eventDate,
    location: data.event.location,
    maxAttendees: data.event.maxAttendees,
    registeredAttendees: data.registrationCount,
    createdBy: data.event.createdBy,
    isFeatured: false, // API doesn't provide this yet
    isUserRegistered: data.isRegistered
  } as Event : undefined;
  
  // Registration mutations
  const { mutate: registerForEvent, isPending: isRegistering } = useRegisterForEvent();
  const { mutate: cancelRegistration, isPending: isCancelling } = useCancelRegistration();
  const { mutate: deleteEvent, isPending: isDeleting } = useDeleteEvent();
  
  // Check if user can edit/delete the event (admin or event creator)
  const canManageEvent = isAuthenticated && 
    event && 
    (role === 'Admin' || role === 'Organizer' || event.createdBy === localStorage.getItem('username'));
  
  // Format the date
  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      
      // Check if date is valid
      if (isNaN(date.getTime())) {
        return 'Date not available';
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
      return 'Date not available';
    }
  };
  
  // Handle registration
  const handleRegister = () => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: `/events/${eventId}` } });
      return;
    }
    
    registerForEvent(eventId, {
      onSuccess: () => {
        // Refetch event details to update UI
        window.location.reload();
      },
    });
  };
  
  // Handle cancellation
  const handleCancel = () => {
    cancelRegistration(eventId, {
      onSuccess: () => {
        // Refetch event details to update UI
        window.location.reload();
      },
    });
  };
  
  // Handle deletion
  const handleDelete = () => {
    deleteEvent(eventId, {
      onSuccess: () => {
        // Navigate back to events list after successful deletion
        navigate('/events');
      },
    });
    setShowDeleteModal(false);
  };
  
  // Check if registration is possible
  const canRegister = isAuthenticated && 
    event && 
    !event.isUserRegistered && 
    (!event.maxAttendees || event.registeredAttendees < event.maxAttendees) &&
    (() => {
      try {
        const eventDate = new Date(event.date);
        return !isNaN(eventDate.getTime()) && eventDate > new Date();
      } catch (error) {
        return false;
      }
    })();
  
  // Check if cancellation is possible
  const canCancel = isAuthenticated && event && event.isUserRegistered;
  
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
        Error loading event: {(error as Error).message}
      </Alert>
    );
  }
  
  if (!event) {
    return (
      <Alert variant="warning">
        Event not found. It may have been removed or you don't have permission to view it.
      </Alert>
    );
  }
  
  return (
    <div>
      <Button 
        variant="outline-secondary" 
        className="mb-3"
        onClick={() => navigate('/events')}
      >
        &larr; Back to Events
      </Button>
      
      <Card className="mb-4">
        <Card.Header className="d-flex justify-content-between align-items-center">
          <h1 className="mb-0">{event.title}</h1>
          <div className="d-flex align-items-center">
            {event.isFeatured && (
              <Badge bg="warning" className="me-1">Featured</Badge>
            )}
            {(() => {
              try {
                const eventDate = new Date(event.date);
                return !isNaN(eventDate.getTime()) && eventDate > new Date() ? (
                  <Badge bg="success" className="me-1">Upcoming</Badge>
                ) : (
                  <Badge bg="secondary" className="me-1">Past</Badge>
                );
              } catch (error) {
                return <Badge bg="secondary" className="me-1">Past</Badge>;
              }
            })()}
            
            {canManageEvent && (
              <div className="ms-2">
                <Button 
                  variant="outline-primary" 
                  size="sm"
                  className="me-1"
                  onClick={() => navigate(`/events/edit/${eventId}`)}
                >
                  Edit
                </Button>
                <Button 
                  variant="outline-danger" 
                  size="sm"
                  onClick={() => setShowDeleteModal(true)}
                >
                  Delete
                </Button>
              </div>
            )}
          </div>
        </Card.Header>
        
        <Card.Body>
          <Row>
            <Col md={8}>
              <Card.Text>{event.description}</Card.Text>
            </Col>
            
            <Col md={4}>
              <Card className="mb-3">
                <Card.Body>
                  <h5>Event Details</h5>
                  <p><strong>Date:</strong> {formatDate(event.date)}</p>
                  {event.location && (
                    <p><strong>Location:</strong> {event.location}</p>
                  )}
                  <p>
                    <strong>Attendees:</strong> {event.registeredAttendees} / {event.maxAttendees || '∞'}
                  </p>
                  <p><strong>Created by:</strong> {event.createdBy}</p>
                  
                  {canRegister && (
                    <Button 
                      variant="primary" 
                      className="w-100"
                      onClick={handleRegister}
                      disabled={isRegistering}
                    >
                      {isRegistering ? 'Registering...' : 'Register for Event'}
                    </Button>
                  )}
                  
                  {canCancel && (
                    <Button 
                      variant="outline-danger" 
                      className="w-100"
                      onClick={handleCancel}
                      disabled={isCancelling}
                    >
                      {isCancelling ? 'Cancelling...' : 'Cancel Registration'}
                    </Button>
                  )}
                  
                  {event.isUserRegistered && (
                    <Alert variant="success" className="mt-3 mb-0">
                      You are registered for this event!
                    </Alert>
                  )}
                  
                  {event.maxAttendees && event.registeredAttendees >= event.maxAttendees && !event.isUserRegistered && (
                    <Alert variant="warning" className="mt-3 mb-0">
                      This event is full.
                    </Alert>
                  )}
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Card.Body>
      </Card>
      {/* Delete Confirmation Modal */}
      <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Deletion</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete the event "{event.title}"? This action cannot be undone.
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>
            Cancel
          </Button>
          <Button 
            variant="danger" 
            onClick={handleDelete}
            disabled={isDeleting}
          >
            {isDeleting ? 'Deleting...' : 'Delete Event'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default EventDetails;
