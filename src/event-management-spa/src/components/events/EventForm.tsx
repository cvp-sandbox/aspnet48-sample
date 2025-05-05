import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, Button, Alert } from 'react-bootstrap';
import { CreateEventRequest, UpdateEventRequest } from '../../api/generated';
import { Event } from '../../types/api';

// Form validation schema
const eventSchema = z.object({
  name: z.string().min(3, 'Name is required'),
  description: z.string().nullable().optional(),
  eventDate: z.string().refine(date => !isNaN(Date.parse(date)), 'Invalid date format'),
  location: z.string().nullable().optional(),
  maxAttendees: z.number().int().positive().optional(),
});

type EventFormValues = z.infer<typeof eventSchema>;

type EventFormProps = {
  event?: Event;
  isEdit?: boolean;
  onSubmit: (data: CreateEventRequest | UpdateEventRequest) => void;
  isSubmitting: boolean;
  error?: Error | null;
};

/**
 * Reusable form component for creating and editing events
 */
const EventForm = ({ event, isEdit = false, onSubmit, isSubmitting, error }: EventFormProps) => {
  const { register, handleSubmit, formState: { errors } } = useForm<EventFormValues>({
    resolver: zodResolver(eventSchema),
    defaultValues: isEdit && event ? {
      name: event.title,
      description: event.description || '',
      eventDate: new Date(event.date).toISOString().split('T')[0],
      location: event.location || '',
      maxAttendees: event.maxAttendees || 0,
    } : undefined
  });

  const onFormSubmit = (data: EventFormValues) => {
    if (isEdit && event) {
      onSubmit({
        eventId: event.eventId,
        name: data.name,
        description: data.description || null,
        eventDate: data.eventDate,
        location: data.location || null,
        maxAttendees: data.maxAttendees || 0
      });
    } else {
      onSubmit({
        name: data.name,
        description: data.description || null,
        eventDate: data.eventDate,
        location: data.location || null,
        maxAttendees: data.maxAttendees || 0
      });
    }
  };

  return (
    <Form onSubmit={handleSubmit(onFormSubmit)}>
      {error && <Alert variant="danger">{error.message}</Alert>}
      
      <Form.Group className="mb-3">
        <Form.Label>Event Name*</Form.Label>
        <Form.Control 
          type="text" 
          {...register('name')} 
          isInvalid={!!errors.name}
        />
        {errors.name && <Form.Control.Feedback type="invalid">{errors.name.message}</Form.Control.Feedback>}
      </Form.Group>
      
      <Form.Group className="mb-3">
        <Form.Label>Description</Form.Label>
        <Form.Control 
          as="textarea" 
          rows={3} 
          {...register('description')} 
          isInvalid={!!errors.description}
        />
        {errors.description && <Form.Control.Feedback type="invalid">{errors.description.message}</Form.Control.Feedback>}
      </Form.Group>
      
      <Form.Group className="mb-3">
        <Form.Label>Event Date*</Form.Label>
        <Form.Control 
          type="date" 
          {...register('eventDate')} 
          isInvalid={!!errors.eventDate}
        />
        {errors.eventDate && <Form.Control.Feedback type="invalid">{errors.eventDate.message}</Form.Control.Feedback>}
      </Form.Group>
      
      <Form.Group className="mb-3">
        <Form.Label>Location</Form.Label>
        <Form.Control 
          type="text" 
          {...register('location')} 
          isInvalid={!!errors.location}
        />
        {errors.location && <Form.Control.Feedback type="invalid">{errors.location.message}</Form.Control.Feedback>}
      </Form.Group>
      
      <Form.Group className="mb-3">
        <Form.Label>Maximum Attendees</Form.Label>
        <Form.Control 
          type="number" 
          min="1"
          {...register('maxAttendees', { valueAsNumber: true })} 
          isInvalid={!!errors.maxAttendees}
        />
        {errors.maxAttendees && <Form.Control.Feedback type="invalid">{errors.maxAttendees.message}</Form.Control.Feedback>}
      </Form.Group>
      
      <Button type="submit" variant="primary" disabled={isSubmitting}>
        {isSubmitting ? 'Saving...' : isEdit ? 'Update Event' : 'Create Event'}
      </Button>
    </Form>
  );
};

export default EventForm;
