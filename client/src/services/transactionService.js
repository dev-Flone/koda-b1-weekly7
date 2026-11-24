import { apiRequest, unwrapCollection } from "./api";

export async function getTransactionsByWalletId(walletId, accessToken) {
  const data = await apiRequest(`/Transaction/wallet/${walletId}`, accessToken);

  return unwrapCollection(data);
}
