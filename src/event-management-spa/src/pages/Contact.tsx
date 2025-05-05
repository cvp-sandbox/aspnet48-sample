import React from 'react';
import { Row, Col, Card, Form, Button } from 'react-bootstrap';
import { FaEnvelope, FaPhone, FaMapMarkerAlt } from 'react-icons/fa';

/**
 * Contact page component
 */
const Contact = () => {
  // Handle form submission
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // In a real app, this would send the form data to a backend API
    alert('Thank you for your message! We will get back to you soon.');
  };

  return (
    <>
      <Row className="mb-4">
        <Col>
          <h1>Contact Us</h1>
          <p className="lead">
            Have questions or feedback? We'd love to hear from you.
          </p>
        </Col>
      </Row>
      
      <Row>
        <Col lg={6} className="mb-4">
          <Card className="h-100">
            <Card.Body>
              <h2>Send us a message</h2>
              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="formName">
                  <Form.Label>Name</Form.Label>
                  <Form.Control type="text" placeholder="Enter your name" required />
                </Form.Group>
                
                <Form.Group className="mb-3" controlId="formEmail">
                  <Form.Label>Email address</Form.Label>
                  <Form.Control type="email" placeholder="Enter your email" required />
                  <Form.Text className="text-muted">
                    We'll never share your email with anyone else.
                  </Form.Text>
                </Form.Group>
                
                <Form.Group className="mb-3" controlId="formSubject">
                  <Form.Label>Subject</Form.Label>
                  <Form.Control type="text" placeholder="Enter subject" required />
                </Form.Group>
                
                <Form.Group className="mb-3" controlId="formMessage">
                  <Form.Label>Message</Form.Label>
                  <Form.Control as="textarea" rows={5} placeholder="Enter your message" required />
                </Form.Group>
                
                <Button variant="primary" type="submit">
                  Send Message
                </Button>
              </Form>
            </Card.Body>
          </Card>
        </Col>
        
        <Col lg={6} className="mb-4">
          <Card className="mb-4">
            <Card.Body>
              <h2>Contact Information</h2>
              <div className="d-flex align-items-center mb-3">
                <FaEnvelope className="me-3 text-primary" size={24} />
                <div>
                  <h5 className="mb-0">Email</h5>
                  <p className="mb-0">contact@dotnetevents.com</p>
                </div>
              </div>
              
              <div className="d-flex align-items-center mb-3">
                <FaPhone className="me-3 text-primary" size={24} />
                <div>
                  <h5 className="mb-0">Phone</h5>
                  <p className="mb-0">(555) 123-4567</p>
                </div>
              </div>
              
              <div className="d-flex align-items-center">
                <FaMapMarkerAlt className="me-3 text-primary" size={24} />
                <div>
                  <h5 className="mb-0">Address</h5>
                  <p className="mb-0">
                    123 Developer Way<br />
                    Redmond, WA 98052
                  </p>
                </div>
              </div>
            </Card.Body>
          </Card>
          
          <Card>
            <Card.Body>
              <h2>Office Hours</h2>
              <ul className="list-unstyled">
                <li><strong>Monday - Friday:</strong> 9:00 AM - 5:00 PM</li>
                <li><strong>Saturday:</strong> 10:00 AM - 2:00 PM</li>
                <li><strong>Sunday:</strong> Closed</li>
              </ul>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </>
  );
};

export default Contact;
