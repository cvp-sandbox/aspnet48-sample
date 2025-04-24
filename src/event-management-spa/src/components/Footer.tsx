import { Container } from 'react-bootstrap';

/**
 * Footer component
 */
const Footer = () => {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="bg-dark text-light py-4 mt-auto">
      <Container>
        <div className="d-flex flex-wrap justify-content-between align-items-center">
          <div className="col-md-4 d-flex align-items-center">
            <span className="mb-3 mb-md-0">
              &copy; {currentYear} Event Management System
            </span>
          </div>
          
          <ul className="nav col-md-4 justify-content-end list-unstyled d-flex">
            <li className="ms-3">
              <a className="text-light" href="#">
                Terms of Service
              </a>
            </li>
            <li className="ms-3">
              <a className="text-light" href="#">
                Privacy Policy
              </a>
            </li>
            <li className="ms-3">
              <a className="text-light" href="#">
                Contact
              </a>
            </li>
          </ul>
        </div>
      </Container>
    </footer>
  );
};

export default Footer;
