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
- [x] Set up request/response interceptors for authentication headers (X-Username, X-Role)
- [x] Implement error handling middleware

### Authentication Infrastructure
- [x] Create AuthContext for global auth state
- [x] Implement authentication header mechanism
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
- [ ] Implement LoginStatus component

### GET Operations (Priority)
- [ ] Implement EventList component using `GET /api/events`
- [ ] Create EventCard component for list items
- [ ] Create EventDetails component using `GET /api/events/{id}`
- [ ] Implement MyEvents component using `GET /api/events/my-events`
- [ ] Create MyRegistrations component using `GET /api/events/my-registrations`
- [ ] Implement FeaturedEvents component using `GET /api/events/featured`
- [ ] Create EventStats component using `GET /api/events/stats`
- [ ] Implement UpcomingEvents component using `GET /api/events/upcoming`

### POST/PUT/DELETE Operations (Secondary)
- [ ] Create EventForm component with validation
- [ ] Implement event creation using `POST /api/events`
- [ ] Implement event editing using `PUT /api/events/{id}`
- [ ] Create event deletion using `DELETE /api/events/{id}`
- [ ] Implement registration functionality using `POST /api/events/{id}/register`
- [ ] Create registration cancellation using `POST /api/events/{id}/cancel-registration`

## Phase 3: Authentication Features

### User Authentication
- [ ] Implement login form using `POST /api/users/login`
- [ ] Create registration form using `POST /api/users/register`
- [ ] Implement logout functionality using `POST /api/users/logoff`
- [ ] Add role-based conditional rendering

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

// Request interceptor to add auth headers
apiClient.interceptors.request.use(
  (config) => {
    const auth = useContext(AuthContext);
    if (auth.isAuthenticated) {
      config.headers['X-Username'] = auth.username;
      if (auth.roles && auth.roles.length > 0) {
        config.headers['X-Role'] = auth.roles;
      }
    }
    return config;
  },
  (error) => {
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
| Layout Components | 75% | In Progress |
| GET Operations | 0% | Not Started |
| POST/PUT/DELETE Operations | 0% | Not Started |
| Testing | 0% | Not Started |
| Overall | 30% | Phase 1 Completed, Phase 2 Started |

## Notes

- The React SPA will use the existing .NET 9.0 API as a backend for frontend (BFF)
- API base URL: https://localhost:7264/
- Authentication will be handled via custom headers (X-Username, X-Role)
- GET operations are prioritized for initial implementation
