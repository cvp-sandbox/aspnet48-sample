import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
  requiredRole?: string;
}

/**
 * A component that protects routes based on authentication and role
 * @param requiredRole - Optional role required to access the route
 */
const ProtectedRoute = ({ requiredRole }: ProtectedRouteProps) => {
  const { isAuthenticated, role } = useAuth();

  // If not authenticated, redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // If a specific role is required and the user doesn't have it, redirect to access denied
  if (requiredRole && role !== requiredRole) {
    return <Navigate to="/access-denied" replace />;
  }

  // If authenticated and has the required role (or no specific role is required), render the child routes
  return <Outlet />;
};

export default ProtectedRoute;
