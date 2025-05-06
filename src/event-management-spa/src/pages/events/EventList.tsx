import { useState } from 'react';
import { Row, Col, Form, InputGroup, Button, Alert, Spinner } from 'react-bootstrap';
import { FaSearch, FaPlus } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { useEvents } from '../../hooks/useEvents';
import { useAuth } from '../../contexts/AuthContext';
import EventCard from './EventCard';
import { Event } from '../../types/api';

/**
 * Component that displays a list of events with search and filtering capabilities
 */
const EventList = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [showUpcomingOnly, setShowUpcomingOnly] = useState(false);
  const navigate = useNavigate();
  const { isAuthenticated, role } = useAuth();
  
  // Check if user can create events (admin or organizer)
  const canCreateEvent = isAuthenticated && (role === 'Admin' || role === 'Organizer');
  
  // Fetch events using the useEvents hook
  const { data, isLoading, error } = useEvents();
  
  // Filter events based on search term and upcoming filter
  const filteredEvents = data?.events?.filter((event: Event) => {
    // Filter by search term (case insensitive)
    const matchesSearch = !searchTerm || 
      event.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (event.description && event.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (event.location && event.location.toLowerCase().includes(searchTerm.toLowerCase()));
    
    // Filter by upcoming status if the checkbox is checked
    const matchesUpcoming = !showUpcomingOnly || new Date(event.date) > new Date();
    
    return matchesSearch && matchesUpcoming;
  }) || [];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="mb-0">Events</h1>
        {canCreateEvent && (
          <Button 
            variant="primary"
            onClick={() => navigate('/events/create')}
          >
            <FaPlus className="me-1" /> Create Event
          </Button>
        )}
      </div>
      
      {/* Search and filter section */}
      <div className="search-container mb-4">
        <Row>
          <Col md={8}>
            <InputGroup>
              <Form.Control
                placeholder="Search events..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              <Button variant="outline-secondary">
                <FaSearch />
              </Button>
            </InputGroup>
          </Col>
          <Col md={4} className="d-flex align-items-center mt-3 mt-md-0">
            <Form.Check
              type="checkbox"
              id="upcoming-only"
              label="Show upcoming events only"
              checked={showUpcomingOnly}
              onChange={(e) => setShowUpcomingOnly(e.target.checked)}
            />
          </Col>
        </Row>
      </div>
      
      {/* Loading state */}
      {isLoading && (
        <div className="text-center my-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        </div>
      )}
      
      {/* Error state */}
      {error && (
        <Alert variant="danger">
          Error loading events: {(error as Error).message}
        </Alert>
      )}
      
      {/* No results state */}
      {!isLoading && !error && filteredEvents.length === 0 && (
        <Alert variant="info">
          No events found. {searchTerm && 'Try adjusting your search criteria.'}
        </Alert>
      )}
      
      {/* Events grid */}
      <Row xs={1} md={2} lg={3} className="g-4">
        {filteredEvents.map((event: Event) => (
          <Col key={event.eventId}>
            <EventCard event={event} />
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default EventList;
