import { apiRequest } from "./api";

export function getUserById(userId) {
  return apiRequest(`/Users/${userId}`);
}
