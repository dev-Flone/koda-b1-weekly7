import { apiRequest } from "./api";

export const createTransfer = (request, accessToken) => {
  return apiRequest("/transfer", {
    method: "POST",
    accessToken,
    body: JSON.stringify(request),
  });
};
