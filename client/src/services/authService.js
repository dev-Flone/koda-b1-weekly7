import { apiRequest } from "./api";

export function login(credentials) {
  return apiRequest("/Auth/login", {
    method: "POST",
    body: JSON.stringify(credentials),
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
