import { apiRequest } from "./api";

export function getMyWallet(accessToken) {
  return apiRequest("/wallets/me", { accessToken });
}
