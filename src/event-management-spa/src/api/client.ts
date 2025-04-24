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
    // Get auth data from localStorage
    const username = localStorage.getItem('username');
    const role = localStorage.getItem('role');

    // Add auth headers if available
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
