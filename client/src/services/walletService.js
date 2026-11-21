import { apiRequest } from "./api";

export function getWalletById(walletId) {
  return apiRequest(`/Wallets/${walletId}`);
}
