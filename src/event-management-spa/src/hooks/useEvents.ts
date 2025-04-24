import { useApiQuery, useApiMutation } from './useApi';
import { EventsService, CreateEventRequest, UpdateEventRequest } from '../api/generated';

/**
 * Custom hook for fetching all events
 */
export const useEvents = () => {
  return useApiQuery(['events'], () => EventsService.getAllEvents());
};

/**
 * Custom hook for fetching a single event by ID
 * @param id - The ID of the event to fetch
 */
export const useEventById = (id: number) => {
  return useApiQuery(['events', id], () => EventsService.getEventById(id), {
    enabled: !!id, // Only run the query if id is provided
  });
};

/**
 * Custom hook for fetching events created by the current user
 */
export const useMyEvents = () => {
  return useApiQuery(['myEvents'], () => EventsService.getEventsByCreator());
};

/**
 * Custom hook for fetching events the current user is registered for
 */
export const useMyRegistrations = () => {
  return useApiQuery(['myRegistrations'], () => EventsService.getRegistrationsByUserId());
};

/**
 * Custom hook for fetching featured events
 */
export const useFeaturedEvents = () => {
  return useApiQuery(['featuredEvents'], () => EventsService.getFeaturedEvents());
};

/**
 * Custom hook for fetching event statistics
 */
export const useEventStats = () => {
  return useApiQuery(['eventStats'], () => EventsService.getStats());
};

/**
 * Custom hook for fetching upcoming events
 */
export const useUpcomingEvents = () => {
  return useApiQuery(['upcomingEvents'], () => EventsService.getUpcomingEvents());
};

/**
 * Custom hook for creating a new event
 */
export const useCreateEvent = () => {
  return useApiMutation<any, CreateEventRequest>(
    (eventData) => EventsService.createEvent(eventData)
  );
};

/**
 * Custom hook for updating an existing event
 */
export const useUpdateEvent = () => {
  return useApiMutation<any, UpdateEventRequest>(
    (eventData) => EventsService.updateEvent(eventData.eventId, eventData)
  );
};

/**
 * Custom hook for deleting an event
 */
export const useDeleteEvent = () => {
  return useApiMutation<any, number>(
    (eventId) => EventsService.deleteEvent(eventId)
  );
};

/**
 * Custom hook for registering for an event
 */
export const useRegisterForEvent = () => {
  return useApiMutation<any, number>(
    (eventId) => EventsService.registerForEvent(eventId)
  );
};

/**
 * Custom hook for canceling registration for an event
 */
export const useCancelRegistration = () => {
  return useApiMutation<any, number>(
    (eventId) => EventsService.cancelRegistration(eventId)
  );
};
