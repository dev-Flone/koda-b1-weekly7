import { apiRequest } from "./api";

export function getActivePaymentMethods() {
  return apiRequest("/PaymentMethod/active");
}

export function createTopUp(request) {
  return apiRequest("/TopUp", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getTopUpsByWalletId(walletId) {
  return apiRequest(`/TopUp/wallet/${walletId}`);
}

export function getTopUpById(id) {
  return apiRequest(`/TopUp/${id}`);
}

export function confirmTopUp(id) {
  return apiRequest(`/TopUp/${id}/confirm`, {
    method: "POST",
  });
}
