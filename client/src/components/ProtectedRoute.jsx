import { Navigate, Outlet } from "react-router";
import { useAuth } from "../context/useAuth";

const ProtectedRoute = () => {
  const { isAuthenticated, isInitializing } = useAuth();

  if (isInitializing) return <p>Memulihkan sesi...</p>;

  if (!isAuthenticated) return <Navigate to="/signin" replace />;
  return <Outlet />;
};

export default ProtectedRoute;
