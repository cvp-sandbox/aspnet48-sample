/**
 * Interface for Event data
 */
export interface Event {
  eventId: number;
  title: string;
  description?: string;
  date: string;
  location?: string;
  maxAttendees?: number;
  registeredAttendees: number;
  createdBy: string;
  isFeatured: boolean;
  isUserRegistered?: boolean;
}

/**
 * Interface for Event list response
 */
export interface EventListResponse {
  events: Event[];
  totalCount: number;
}

/**
 * Interface for Event statistics
 */
export interface EventStats {
  totalEvents: number;
  upcomingEvents: number;
  pastEvents: number;
  totalRegistrations: number;
  featuredEvents: number;
}
