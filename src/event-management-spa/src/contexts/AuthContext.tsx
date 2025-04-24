import { createContext, useContext, useState, ReactNode, useEffect } from 'react';

// Define the shape of the auth state
interface AuthState {
  isAuthenticated: boolean;
  username: string | null;
  role: string | null;
}

// Define the shape of the auth context
interface AuthContextType extends AuthState {
  login: (username: string, role: string) => void;
  logout: () => void;
}

// Create the auth context with default values
const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  username: null,
  role: null,
  login: () => {},
  logout: () => {},
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
  });

  // Load auth state from localStorage on component mount
  useEffect(() => {
    const username = localStorage.getItem('username');
    const role = localStorage.getItem('role');

    if (username && role) {
      setAuthState({
        isAuthenticated: true,
        username,
        role,
      });
    }
  }, []);

  // Login function
  const login = (username: string, role: string) => {
    // Save auth data to localStorage
    localStorage.setItem('username', username);
    localStorage.setItem('role', role);

    // Update auth state
    setAuthState({
      isAuthenticated: true,
      username,
      role,
    });
  };

  // Logout function
  const logout = () => {
    // Remove auth data from localStorage
    localStorage.removeItem('username');
    localStorage.removeItem('role');

    // Update auth state
    setAuthState({
      isAuthenticated: false,
      username: null,
      role: null,
    });
  };

  // Provide auth context to children
  return (
    <AuthContext.Provider
      value={{
        ...authState,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
