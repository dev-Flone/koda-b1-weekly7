import { useEffect, useRef, useState } from "react";
import { AuthContext } from "./authContext";
import {
  login as loginRequest,
  logoutRequest,
  refreshAccessToken,
} from "../services/authService";
import { setAccessTokenGetter, setAccessTokenUpdater } from "../services/api";

const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(null);
  const [user, setUser] = useState(null);
  const [isInitializing, setIsInitializing] = useState(true);

  const accessTokenRef = useRef(accessToken);

  // Selalu sinkronkan ref dengan state terbaru
  const updateAccessToken = (token) => {
    accessTokenRef.current = token;
    setAccessToken(token);
  };

  // Hubungkan AuthProvider dengan api.js
  useEffect(() => {
    setAccessTokenGetter(() => accessTokenRef.current);

    setAccessTokenUpdater((token) => {
      setAccessToken(token);

      // Jika token null, sesi pengguna juga harus dibersihkan
      if (!token) {
        setUser(null);
      }
    });
  }, []);

  useEffect(() => {
    const restoreSession = async () => {
      try {
        const response = await refreshAccessToken();

        setAccessToken(response.accessToken);

        setUser({
          userId: response.userId,
          userName: response.userName,
          fullName: response.fullName,
          email: response.email,
        });
      } catch {
        setAccessToken(null);
        setUser(null);
      } finally {
        setIsInitializing(false);
      }
    };
    restoreSession();
  }, []);

  const login = async (credentials) => {
    const response = await loginRequest(credentials);

    setAccessToken(response.accessToken);

    setUser({
      userId: response.userId,
      userName: response.userName,
      fullName: response.fullName,
      email: response.email,
    });

    return response;
  };

  const logout = async () => {
    try {
      await logoutRequest();
    } finally {
      setAccessToken(null);
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
