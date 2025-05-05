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

  console.log('ProtectedRoute - isAuthenticated:', isAuthenticated);
  console.log('ProtectedRoute - user role:', role);
  console.log('ProtectedRoute - required role:', requiredRole);

  // If not authenticated, redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // If a specific role is required, check if the user has the required role or is an Admin
  if (requiredRole && role !== requiredRole && role !== 'Admin') {
    console.log('Access denied - user does not have required role');
    return <Navigate to="/access-denied" replace />;
  }

  // If authenticated and has the required role (or is an Admin, or no specific role is required), render the child routes
  return <Outlet />;
};

export default ProtectedRoute;
