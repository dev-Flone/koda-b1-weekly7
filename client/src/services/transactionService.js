import { apiRequest, unwrapCollection } from "./api";

export async function getTransactionsByWalletId(walletId) {
  const data = await apiRequest(`/Transaction/wallet/${walletId}`);

  return unwrapCollection(data);
}
