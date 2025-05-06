import { Container } from 'react-bootstrap';

/**
 * Footer component
 */
const Footer = () => {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="mt-auto">
      <Container>
        <hr />
        <p>&copy; {currentYear} - .NET Events</p>
      </Container>
    </footer>
  );
};

export default Footer;
