import { Nav, NavDropdown } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useAuthentication } from '../hooks/useAuth';

/**
 * Component that displays the current login status and provides login/logout functionality
 */
const LoginStatus = () => {
  const { isAuthenticated, username, role } = useAuth();
  const { logout } = useAuthentication();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Nav>
      {isAuthenticated ? (
        <NavDropdown title={username || 'User'} id="user-dropdown" align="end">
          <NavDropdown.Item as={Link} to="/profile">Profile</NavDropdown.Item>
          {role === 'Admin' && (
            <>
              <NavDropdown.Item as={Link} to="/admin/events">Manage Events</NavDropdown.Item>
              <NavDropdown.Item as={Link} to="/admin/users">Manage Users</NavDropdown.Item>
            </>
          )}
          <NavDropdown.Divider />
          <NavDropdown.Item onClick={handleLogout}>Logout</NavDropdown.Item>
        </NavDropdown>
      ) : (
        <>
          <Nav.Link as={Link} to="/login">Login</Nav.Link>
          <Nav.Link as={Link} to="/register">Register</Nav.Link>
        </>
      )}
    </Nav>
  );
};

export default LoginStatus;
