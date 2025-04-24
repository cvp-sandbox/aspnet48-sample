import { useApiMutation } from './useApi';
import { EventManagementApiService, LoginRequest, RegisterRequest } from '../api/generated';
import { useAuth as useAuthContext } from '../contexts/AuthContext';

/**
 * Custom hook for authentication operations
 */
export const useAuthentication = () => {
  const { login: setAuth, logout: clearAuth } = useAuthContext();

  // Login mutation
  const loginMutation = useApiMutation<any, LoginRequest>(
    (credentials) => EventManagementApiService.login(credentials),
    {
      onSuccess: (data) => {
        // Assuming the API returns username and role information
        // You might need to adjust this based on the actual API response
        if (data && data.username && data.role) {
          setAuth(data.username, data.role);
        }
      },
    }
  );

  // Register mutation
  const registerMutation = useApiMutation<any, RegisterRequest>(
    (userData) => EventManagementApiService.register(userData)
  );

  // Logout mutation
  const logoutMutation = useApiMutation<any, void>(
    () => EventManagementApiService.logOff(),
    {
      onSuccess: () => {
        clearAuth();
      },
    }
  );

  return {
    login: loginMutation.mutate,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,
    
    register: registerMutation.mutate,
    isRegistering: registerMutation.isPending,
    registerError: registerMutation.error,
    
    logout: logoutMutation.mutate,
    isLoggingOut: logoutMutation.isPending,
  };
};
