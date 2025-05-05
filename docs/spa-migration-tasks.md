# Event Registration System - SPA Migration Tasks

This document tracks the tasks required to migrate the ASP.NET MVC Event Registration System to a React SPA.

## Phase 1: Setup & Core Infrastructure

### Project Setup
- [x] Initialize React+Vite project (already done)
- [x] Configure TypeScript settings
- [x] Set up ESLint and Prettier
- [x] Configure directory structure
- [x] Add .env file support for environment variables

### Dependencies Installation
- [x] Install routing dependencies (react-router-dom)
- [x] Install HTTP client (axios)
- [x] Install form handling libraries (react-hook-form, zod)
- [x] Install data fetching library (@tanstack/react-query)
- [x] Install UI libraries (bootstrap, react-bootstrap, react-icons)
- [x] Install OpenAPI code generation tools (openapi-typescript-codegen)
- [x] Install testing libraries (vitest, @testing-library/react, msw)

### API Integration
- [x] Generate TypeScript interfaces from OpenAPI spec
- [x] Configure API base URL (https://localhost:7264/)
- [x] Create API client with Axios
- [x] Set up request/response interceptors for JWT authentication (Authorization header)
- [x] Implement error handling middleware

### Authentication Infrastructure
- [x] Create AuthContext for global auth state
- [x] Implement JWT token management
- [x] Create login/logout functionality
- [x] Create protected route component

### Routing Setup
- [x] Set up React Router
- [x] Define route configuration for API-supported views
- [x] Implement role-based route protection

## Phase 2: Core Features Implementation (Prioritizing GET Operations)

### Layout Components
- [x] Create Layout component
- [x] Implement Header component with navigation
- [x] Create Footer component
- [x] Implement LoginStatus component

### GET Operations (Priority)
- [x] Implement EventList component using `GET /api/events`
- [x] Create EventCard component for list items
- [x] Create EventDetails component using `GET /api/events/{id}`
- [x] Implement MyEvents component using `GET /api/events/my-events`
- [x] Create MyRegistrations component using `GET /api/events/my-registrations`
- [x] Implement FeaturedEvents component using `GET /api/events/featured`
- [x] Create EventStats component using `GET /api/events/stats`
- [x] Implement UpcomingEvents component using `GET /api/events/upcoming`

### POST/PUT/DELETE Operations (Secondary)
- [ ] Create EventForm component with validation
- [ ] Implement event creation using `POST /api/events`
- [ ] Implement event editing using `PUT /api/events/{id}`
- [ ] Create event deletion using `DELETE /api/events/{id}`
- [ ] Implement registration functionality using `POST /api/events/{id}/register`
- [ ] Create registration cancellation using `POST /api/events/{id}/cancel-registration`

## Phase 3: Authentication Features

### User Authentication
- [x] Implement login form using `POST /api/users/login`
- [x] Create registration form using `POST /api/users/register`
- [x] Implement logout functionality using `POST /api/users/logoff`
- [x] Add role-based conditional rendering

## Phase 4: Testing & Refinement

### Component Testing
- [ ] Set up testing environment
- [ ] Create tests for API-integrated components
- [ ] Test protected routes

### UI/UX Refinement
- [ ] Ensure responsive design
- [ ] Implement loading states
- [ ] Add error handling UI
- [ ] Improve form validation feedback

### Performance Optimization
- [ ] Implement code splitting
- [ ] Add caching strategies
- [ ] Implement lazy loading for routes

## Phase 5: Deployment & Documentation

### Build Configuration
- [ ] Configure production build settings
- [ ] Set up environment-specific variables

### Documentation
- [ ] Create README with setup instructions
- [ ] Document component usage
- [ ] Add API integration documentation

## Implementation Details

### OpenAPI Code Generation

We'll use `openapi-typescript-codegen` to generate TypeScript interfaces and API client from the OpenAPI spec:

```bash
npm install openapi-typescript-codegen --save-dev
```

Generate API client:

```bash
npx openapi-typescript-codegen --input src/EventManagement.Api/openAPI-v1.json --output src/event-management-spa/src/api --client axios
```

### API Client Structure

```typescript
// src/api/client.ts
import axios from 'axios';
import { AuthContext } from '../contexts/AuthContext';

const apiClient = axios.create({
  baseURL: 'https://localhost:7264',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add JWT auth header
apiClient.interceptors.request.use(
  (config) => {
    const auth = useContext(AuthContext);
    if (auth.isAuthenticated && auth.token) {
      config.headers['Authorization'] = `Bearer ${auth.token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle token expiration
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // If error is 401 and we haven't already tried to refresh
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        // Attempt to refresh the token
        const auth = useContext(AuthContext);
        await auth.refreshToken();
        
        // Retry the original request with new token
        if (auth.token) {
          originalRequest.headers['Authorization'] = `Bearer ${auth.token}`;
          return axios(originalRequest);
        }
      } catch (refreshError) {
        // If refresh fails, logout user
        const auth = useContext(AuthContext);
        auth.logout();
        return Promise.reject(refreshError);
      }
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
```

### React Query Integration

```typescript
// src/hooks/useEvents.ts
import { useQuery } from '@tanstack/react-query';
import { EventsService } from '../api';

export const useEvents = () => {
  return useQuery({
    queryKey: ['events'],
    queryFn: () => EventsService.getAllEvents(),
  });
};

export const useEventById = (id: number) => {
  return useQuery({
    queryKey: ['events', id],
    queryFn: () => EventsService.getEventById(id),
    enabled: !!id,
  });
};

// Additional hooks for other GET operations
```

### Component Example (EventList)

```tsx
// src/components/events/EventList.tsx
import { useEvents } from '../hooks/useEvents';
import EventCard from './EventCard';

const EventList = () => {
  const { data, isLoading, error } = useEvents();

  if (isLoading) return <div>Loading events...</div>;
  if (error) return <div>Error loading events: {error.message}</div>;

  return (
    <div className="row">
      {data?.events.map((event) => (
        <div className="col-md-4" key={event.eventId}>
          <EventCard event={event} />
        </div>
      ))}
    </div>
  );
};

export default EventList;
```

## Progress Tracking

| Feature | Progress | Status |
|---------|----------|--------|
| Project Setup | 100% | Completed |
| API Integration | 100% | Completed |
| Authentication | 100% | Completed |
| Layout Components | 100% | Completed |
| GET Operations | 100% | Completed |
| POST/PUT/DELETE Operations | 0% | Not Started |
| Testing | 0% | Not Started |
| Overall | 75% | Phase 1, 2 & 3 Completed, Phase 4 Next |

## Notes

- The React SPA will use the existing .NET 9.0 API as a backend for frontend (BFF)
- API base URL: https://localhost:7264/
- Authentication will be handled via JWT tokens in the Authorization header
- GET operations are prioritized for initial implementation
- Authentication features are now fully implemented with login/register forms, logout functionality, and role-based conditional rendering
- User authentication uses JWT tokens with the API endpoints `/api/users/login`, `/api/users/register`, and `/api/users/logoff`
