import { apiRequest } from "./api";

export function login(credentials) {
  return apiRequest("/Auth/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
}

export function register(data) {
  return apiRequest("/Auth/register", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export function refreshAccessToken() {
  return apiRequest("/Auth/refresh", {
    method: "POST",
  });
}

export function logoutRequest() {
  return apiRequest("/Auth/logout", {
    method: "POST",
  });
}
