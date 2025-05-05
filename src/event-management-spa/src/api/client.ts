import axios from 'axios';

// Create an Axios instance with default configuration
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth headers
apiClient.interceptors.request.use(
  (config) => {
    // Get auth token from localStorage
    const token = localStorage.getItem('auth_token');
    
    // Add Authorization header if token is available
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    
    // For backward compatibility, also add the username and role headers
    const username = localStorage.getItem('username');
    const role = localStorage.getItem('role');

    if (username) {
      config.headers['X-Username'] = username;
    }
    
    if (role) {
      config.headers['X-Role'] = role;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Handle common errors here
    if (error.response) {
      // Server responded with an error status code
      const { status } = error.response;
      
      if (status === 401) {
        // Unauthorized - clear auth data and redirect to login
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        localStorage.removeItem('auth_token');
        window.location.href = '/login';
      }
      
      if (status === 403) {
        // Forbidden - redirect to access denied page
        window.location.href = '/access-denied';
      }
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
