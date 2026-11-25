import { useEffect, useRef, useState } from "react";
import { AuthContext } from "./authContext";
import {
  login as loginRequest,
  logoutRequest,
  refreshAccessToken,
} from "../services/authService";
import { setAccessTokenGetter, setAccessTokenUpdater } from "../services/api";

const mapUser = (response) => ({
  userId: response.userId,
  userName: response.userName,
  fullName: response.fullName,
  email: response.email,
  role: response.role,
});

const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(null);
  const [user, setUser] = useState(null);
  const [isInitializing, setIsInitializing] = useState(true);

  const accessTokenRef = useRef(accessToken);

  const updateAccessToken = (token) => {
    accessTokenRef.current = token;
    setAccessToken(token);
  };

  useEffect(() => {
    setAccessTokenGetter(() => accessTokenRef.current);

    setAccessTokenUpdater((token) => {
      setAccessToken(token);

      if (!token) {
        setUser(null);
      }
    });
  }, []);

  useEffect(() => {
    const restoreSession = async () => {
      try {
        const response = await refreshAccessToken();

        updateAccessToken(response.accessToken);

        setUser(mapUser(response));
      } catch {
        updateAccessToken(null);
        setUser(null);
      } finally {
        setIsInitializing(false);
      }
    };
    restoreSession();
  }, []);

  const login = async (credentials) => {
    const response = await loginRequest(credentials);

    updateAccessToken(response.accessToken);

    setUser(mapUser(response));

    return response;
  };

  const logout = async () => {
    try {
      await logoutRequest();
    } finally {
      updateAccessToken(null);
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        accessToken,
        user,
        isAuthenticated: Boolean(accessToken),
        isInitializing,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
