import { Container, Nav, Navbar, NavDropdown } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import LoginStatus from './LoginStatus';

/**
 * Header component with navigation
 */
const Header = () => {
  const { isAuthenticated, role } = useAuth();

  return (
    <Navbar bg="dark" variant="dark" expand="lg" className="mb-3">
      <Container>
        <Navbar.Brand as={Link} to="/">.NET Events</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="flex-grow-1">
            <Nav.Link as={Link} to="/">Home</Nav.Link>
            <Nav.Link as={Link} to="/about">About</Nav.Link>
            <Nav.Link as={Link} to="/contact">Contact</Nav.Link>
            <Nav.Link as={Link} to="/events">All Events</Nav.Link>
            
            {/* Show these links only when authenticated */}
            {isAuthenticated && (
              <>
                <Nav.Link as={Link} to="/my-events">My Events</Nav.Link>
                <Nav.Link as={Link} to="/my-registrations">My Registrations</Nav.Link>
              </>
            )}
            
            {/* Admin-only links - commented out as in legacy app */}
            {/* 
            {role === 'Admin' && (
              <Nav.Link as={Link} to="/admin/users">Admin</Nav.Link>
            )}
            */}
          </Nav>
          
          <LoginStatus />
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default Header;
