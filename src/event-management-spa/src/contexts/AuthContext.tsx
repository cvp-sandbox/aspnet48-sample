import { createContext, useContext, useState, ReactNode, useEffect } from 'react';

// Define the shape of the auth state
interface AuthState {
  isAuthenticated: boolean;
  username: string | null;
  role: string | null;
  token: string | null;
}

// Define the shape of the auth context
interface AuthContextType extends AuthState {
  login: (username: string, role: string, token?: string) => void;
  logout: () => void;
  getToken: () => string | null;
}

// Create the auth context with default values
const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  username: null,
  role: null,
  token: null,
  login: () => {},
  logout: () => {},
  getToken: () => null,
});

// Custom hook to use the auth context
export const useAuth = () => useContext(AuthContext);

interface AuthProviderProps {
  children: ReactNode;
}

// Auth provider component
export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    username: null,
    role: null,
    token: null,
  });

  // Load auth state from localStorage on component mount
  useEffect(() => {
    const username = localStorage.getItem('username');
    const role = localStorage.getItem('role');
    const token = localStorage.getItem('auth_token');

    if (username && role) {
      setAuthState({
        isAuthenticated: true,
        username,
        role,
        token,
      });
    }
  }, []);

  // Login function
  const login = (username: string, role: string, token?: string) => {
    // Save auth data to localStorage
    localStorage.setItem('username', username);
    localStorage.setItem('role', role);
    
    // Save token if provided
    if (token) {
      localStorage.setItem('auth_token', token);
    }

    // Update auth state
    setAuthState({
      isAuthenticated: true,
      username,
      role,
      token: token || null,
    });
  };

  // Logout function
  const logout = () => {
    // Remove auth data from localStorage
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('auth_token');

    // Update auth state
    setAuthState({
      isAuthenticated: false,
      username: null,
      role: null,
      token: null,
    });
  };

  // Get token function
  const getToken = () => {
    return authState.token || localStorage.getItem('auth_token');
  };

  // Provide auth context to children
  return (
    <AuthContext.Provider
      value={{
        ...authState,
        login,
        logout,
        getToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
