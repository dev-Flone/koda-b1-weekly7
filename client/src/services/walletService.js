import { apiRequest } from "./api";

export function getMyWallet(accessToken) {
  return apiRequest("/wallets/me", { accessToken });
}

export function getRecipientByEmail(email, accessToken) {
  return apiRequest(`/wallets/recipient/email/${encodeURIComponent(email)}`, {
    accessToken,
  });
}

export function getRecipientByWalletNumber(walletNumber, accessToken) {
  return apiRequest(
    `/wallets/recipient/number/${encodeURIComponent(walletNumber)}`,
    { accessToken },
  );
}
