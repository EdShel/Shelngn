import React, { useCallback, useContext, useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { getCurrentUser } from "../api";

const AuthContext = React.createContext();

export const AuthenticationProvider = ({ children }) => {
  const [isAuthenticated, setAuthenticated] = useState(() => !!localStorage.getItem("accessToken"));
  const [user, setUser] = useState(() => {
    const cacheEntry = localStorage.getItem("userCache");
    if (cacheEntry) {
      return JSON.parse(cacheEntry);
    }
    return null;
  });
  const [isLoading, setLoading] = useState(true);
  const location = useLocation();

  const reloadUser = useCallback(async () => {
    setLoading(true);
    try {
      const userReponse = await getCurrentUser();
      setUser(userReponse);
      console.log('userReponse', userReponse)
      localStorage.setItem("userCache", JSON.stringify(userReponse));
      setLoading(false);
    } catch (ex) {
      setTimeout(reloadUser, 500);
    }
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
      localStorage.removeItem("userCache");
      setUser(null);
      return;
    }
    reloadUser();
  }, [reloadUser, isAuthenticated]);

  useEffect(() => {
    const auth = !!localStorage.getItem("accessToken");
    setAuthenticated(auth);
  }, [location]);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        user,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

const useAuth = () => useContext(AuthContext);

export default useAuth;
