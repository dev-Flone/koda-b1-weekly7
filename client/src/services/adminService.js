import { apiRequest } from "./api";

export function getAdminDashboard(accessToken) {
  return apiRequest("/admin/dashboard", {
    accessToken,
  });
}

export function getAdminUsers(accessToken) {
  return apiRequest("/admin/users", {
    accessToken,
  });
}
