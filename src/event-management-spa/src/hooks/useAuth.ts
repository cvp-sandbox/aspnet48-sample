import { useApiMutation } from './useApi';
import { EventManagementApiService, LoginRequest, RegisterRequest } from '../api/generated';
import { useAuth as useAuthContext } from '../contexts/AuthContext';
import { useMutation } from '@tanstack/react-query';

/**
 * Custom hook for authentication operations
 */
export const useAuthentication = () => {
  const { login: setAuth, logout: clearAuth } = useAuthContext();

  // Real API login mutation
  const loginMutation = useApiMutation<any, LoginRequest>(
    (credentials) => EventManagementApiService.login(credentials),
    {
      onSuccess: (data) => {
        console.log('Login response:', data); // Add logging to see the response
        
        // Set authentication state with the API response data
        // Handle both camelCase and PascalCase property names
        const success = data.success ?? data.Success;
        const email = data.email ?? data.Email;
        const roles = data.roles ?? data.Roles;
        const token = data.token ?? data.Token;
        const errorMessage = data.errorMessage ?? data.ErrorMessage;
        
        if (success) {
          // Check if the user has the Admin role
          const isAdmin = roles && roles.includes('Admin');
          // If the user is an Admin, set the role to Admin
          // Otherwise, get the first role or default to 'User'
          const role = isAdmin ? 'Admin' : (roles && roles.length > 0 ? roles[0] : 'User');
          setAuth(email, role, token);
        } else {
          // Handle unsuccessful login
          throw new Error(errorMessage || 'Login failed');
        }
      }
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

  // Create a wrapper for the login function that handles options correctly
  const login = (credentials: LoginRequest, options?: any) => {
    return loginMutation.mutate(credentials, options);
  };

  return {
    login,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,
    
    register: registerMutation.mutate,
    isRegistering: registerMutation.isPending,
    registerError: registerMutation.error,
    
    logout: logoutMutation.mutate,
    isLoggingOut: logoutMutation.isPending,
  };
};
