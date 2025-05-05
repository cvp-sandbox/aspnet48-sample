import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

// Import pages
import Home from './pages/Home';
import About from './pages/About';
import Contact from './pages/Contact';
import Login from './pages/Login';
import Register from './pages/Register';
import EventList from './pages/events/EventList';
import EventDetails from './pages/events/EventDetails';
import CreateEvent from './pages/events/CreateEvent';
import EditEvent from './pages/events/EditEvent';
import MyEvents from './pages/events/MyEvents';
import MyRegistrations from './pages/events/MyRegistrations';

// Placeholder components for routes (to be implemented later)
const AccessDenied = () => <div className="container mt-4"><h1>Access Denied</h1></div>;
const NotFound = () => <div className="container mt-4"><h1>404 - Not Found</h1></div>;

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      retry: 1,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Router>
          <Routes>
            {/* Public routes */}
            <Route path="/" element={<Layout><Home /></Layout>} />
            <Route path="/about" element={<Layout><About /></Layout>} />
            <Route path="/contact" element={<Layout><Contact /></Layout>} />
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
            
            {/* Organizer routes - require organizer or admin role */}
            <Route element={<ProtectedRoute requiredRole="Organizer" />}>
              <Route path="/events/create" element={<Layout><CreateEvent /></Layout>} />
              <Route path="/events/edit/:id" element={<Layout><EditEvent /></Layout>} />
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
