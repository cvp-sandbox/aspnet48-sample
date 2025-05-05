import React from 'react';
import { Row, Col, Card } from 'react-bootstrap';

/**
 * About page component
 */
const About = () => {
  return (
    <>
      <Row className="mb-4">
        <Col>
          <h1>About .NET Events</h1>
          <p className="lead">
            A platform for discovering and managing local events in the developer community.
          </p>
        </Col>
      </Row>
      
      <Row className="mb-4">
        <Col>
          <Card>
            <Card.Body>
              <h2>Our Mission</h2>
              <p>
                .NET Events is dedicated to connecting developers with valuable learning and networking opportunities.
                We provide a simple platform for event organizers to create and manage events, while making it easy for
                attendees to discover and register for events that interest them.
              </p>
              <p>
                Whether you're looking to learn new skills, share your knowledge, or connect with other developers,
                .NET Events is the place to find and create meaningful community experiences.
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      
      <Row>
        <Col md={6} className="mb-4">
          <Card className="h-100">
            <Card.Body>
              <h3>For Event Organizers</h3>
              <ul>
                <li>Create and manage events easily</li>
                <li>Track registrations and attendance</li>
                <li>Communicate with attendees</li>
                <li>Promote your events to the community</li>
              </ul>
            </Card.Body>
          </Card>
        </Col>
        
        <Col md={6} className="mb-4">
          <Card className="h-100">
            <Card.Body>
              <h3>For Attendees</h3>
              <ul>
                <li>Discover relevant events in your area</li>
                <li>Register with a single click</li>
                <li>Manage your event schedule</li>
                <li>Connect with other attendees</li>
              </ul>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </>
  );
};

export default About;
