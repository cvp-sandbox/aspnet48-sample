import { Card, Row, Col, Alert, Spinner } from 'react-bootstrap';
import { useEventStats } from '../../hooks/useEvents';
import { EventStats as EventStatsType } from '../../types/api';
import { FaCalendarAlt, FaCalendarCheck, FaCalendarTimes, FaUserCheck, FaStar } from 'react-icons/fa';

/**
 * Component that displays event statistics
 */
const EventStats = () => {
  // Fetch event statistics
  const { data, isLoading, error } = useEventStats();
  
  // Map the backend response to our frontend type
  let stats: EventStatsType | null = null;
  
  if (data) {
    // Check if data is in the expected format or needs mapping
    if ('totalEvents' in data) {
      // Data is already in the expected format
      stats = data as EventStatsType;
    } else if ('stats' in data) {
      // Data is in the backend format (array of StatItem objects)
      const statsArray = (data as { stats: Array<{ statLabel: string, statValue: number }> }).stats;
      
      // Create a mapping from backend format to frontend format
      stats = {
        totalEvents: statsArray.find(s => s.statLabel === 'ActiveEvents')?.statValue || 0,
        upcomingEvents: statsArray.find(s => s.statLabel === 'ThisWeeksEvents')?.statValue || 0,
        pastEvents: 0, // Not provided in backend, could calculate if needed
        totalRegistrations: statsArray.find(s => s.statLabel === 'RegisteredUsers')?.statValue || 0,
        featuredEvents: 0, // Not provided in backend, could calculate if needed
      };
      
      console.log('Mapped stats:', stats);
    }
  }
  
  if (isLoading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }
  
  if (error) {
    return (
      <Alert variant="danger">
        Error loading event statistics: {(error as Error).message}
      </Alert>
    );
  }
  
  if (!stats) {
    return (
      <Alert variant="warning">
        No statistics available.
      </Alert>
    );
  }
  
  // Define stat cards to match legacy app (only show the 3 stats that are returned from API)
  const statCards = [
    {
      title: 'Active Events',
      value: stats.totalEvents,
      icon: <FaCalendarAlt size={24} />,
      color: 'primary',
    },
    {
      title: 'Registered Users',
      value: stats.totalRegistrations,
      icon: <FaUserCheck size={24} />,
      color: 'primary',
    },
    {
      title: 'Events This Week',
      value: stats.upcomingEvents,
      icon: <FaCalendarCheck size={24} />,
      color: 'primary',
    },
  ];
  
  return (
    <div className="container mb-5">
      <div className="stats-container">
        <Row>
          {statCards.map((stat, index) => (
            <Col md={4} key={index} className="stat-item">
              <div className="text-center">
                <div className="stat-number text-primary" style={{ fontSize: '2.5rem', fontWeight: 'bold' }}>
                  {stat.value}
                </div>
                <div>{stat.title}</div>
              </div>
            </Col>
          ))}
        </Row>
      </div>
    </div>
  );
};

export default EventStats;
