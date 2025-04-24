import { OpenAPI } from '../api/generated';

// Configure the OpenAPI client with the base URL from environment variables
OpenAPI.BASE = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7264';

// Export the configured OpenAPI instance
export { OpenAPI };
