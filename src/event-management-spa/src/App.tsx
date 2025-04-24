import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      retry: 1,
    },
  },
});

// Placeholder components for routes (to be implemented later)
const Home = () => <div className="container mt-4"><h1>Home Page</h1></div>;
const Login = () => <div className="container mt-4"><h1>Login Page</h1></div>;
const Register = () => <div className="container mt-4"><h1>Register Page</h1></div>;
const EventList = () => <div className="container mt-4"><h1>Event List</h1></div>;
const EventDetails = () => <div className="container mt-4"><h1>Event Details</h1></div>;
const MyEvents = () => <div className="container mt-4"><h1>My Events</h1></div>;
const MyRegistrations = () => <div className="container mt-4"><h1>My Registrations</h1></div>;
const AccessDenied = () => <div className="container mt-4"><h1>Access Denied</h1></div>;
const NotFound = () => <div className="container mt-4"><h1>404 - Not Found</h1></div>;

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Router>
          <Routes>
            {/* Public routes */}
            <Route path="/" element={<Layout><Home /></Layout>} />
            <Route path="/login" element={<Layout><Login /></Layout>} />
            <Route path="/register" element={<Layout><Register /></Layout>} />
            <Route path="/events" element={<Layout><EventList /></Layout>} />
            <Route path="/events/:id" element={<Layout><EventDetails /></Layout>} />
            <Route path="/access-denied" element={<Layout><AccessDenied /></Layout>} />
            
            {/* Protected routes - require authentication */}
            <Route element={<ProtectedRoute />}>
              <Route path="/my-events" element={<Layout><MyEvents /></Layout>} />
              <Route path="/my-registrations" element={<Layout><MyRegistrations /></Layout>} />
            </Route>
            
            {/* Admin routes - require admin role */}
            <Route element={<ProtectedRoute requiredRole="Admin" />}>
              {/* Admin routes will be added here */}
            </Route>
            
            {/* Catch-all route for 404 */}
            <Route path="*" element={<Layout><NotFound /></Layout>} />
          </Routes>
        </Router>
      </AuthProvider>
    </QueryClientProvider>
  );
}

export default App;
