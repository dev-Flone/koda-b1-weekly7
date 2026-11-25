import { Navigate, Outlet } from "react-router";
import { useAuth } from "../context/useAuth";

const RoleProtectedRoute = ({ allowedRole }) => {
  const { user } = useAuth();

  const userRole = user?.role?.toUpperCase();
  const requiredRole = allowedRole.toUpperCase();

  if (userRole !== requiredRole) {
    const redirectTo = userRole === "ADMIN" ? "/ringkasan" : "/";

    return <Navigate to={redirectTo} replace />;
  }

  return <Outlet />;
};

export default RoleProtectedRoute;
