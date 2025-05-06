import { Row, Col, Card, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import FeaturedEvents from './events/FeaturedEvents';
import UpcomingEvents from './events/UpcomingEvents';
import EventStats from './events/EventStats';
import { useAuth } from '../contexts/AuthContext';

/**
 * Home page component
 */
const Home = () => {
  const { isAuthenticated } = useAuth();
  
  return (
    <>
      {/* Hero section */}
      <Row className="mb-5">
        <Col>
          <div className="jumbotron">
            <h1 className="display-4">Welcome to Event Management</h1>
            <p className="lead">
              Discover, register, and manage events all in one place.
            </p>
            <hr className="my-4" />
            <p>
              Browse our upcoming events or create your own event today.
            </p>
            <Button
              as={Link as any}
              to="/events"
              variant="primary"
              size="lg"
              className="me-2"
            >
              Browse Events
            </Button>
            {isAuthenticated && (
              <Button
                as={Link as any}
                to="/events/create"
                variant="outline-primary"
                size="lg"
              >
                Create Event
              </Button>
            )}
          </div>
        </Col>
      </Row>
      
      {/* Featured events section */}
      <Row className="mb-5">
        <Col>
          <FeaturedEvents />
        </Col>
      </Row>
      
      {/* Upcoming events section */}
      <Row className="mb-5">
        <Col>
          <UpcomingEvents />
        </Col>
      </Row>
      
      {/* Stats section */}
      <Row className="mb-5">
        <Col>
          <EventStats />
        </Col>
      </Row>
    </>
  );
};

export default Home;
