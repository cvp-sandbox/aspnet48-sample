import { OpenAPI } from '../api/generated';

// Configure the OpenAPI client with the base URL from environment variables
OpenAPI.BASE = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7264';

// Configure token resolver to use JWT from localStorage
OpenAPI.TOKEN = async () => {
  return localStorage.getItem('auth_token') || '';
};

// Export the configured OpenAPI instance
export { OpenAPI };
