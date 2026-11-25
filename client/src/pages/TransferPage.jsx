import { useEffect, useState } from "react";

import { AppLayout } from "../components/AppLayout";
import { useAuth } from "../context/useAuth";
import { createTransfer } from "../services/transferService";

import {
  getMyWallet,
  getRecipientByEmail,
  getRecipientByWalletNumber,
} from "../services/walletService";

const formatRupiah = (amount) =>
  new Intl.NumberFormat("id-ID", {
    style: "currency",
    currency: "IDR",
    maximumFractionDigits: 0,
  }).format(amount ?? 0);

const TransferPage = () => {
  const { accessToken } = useAuth();

  const [wallet, setWallet] = useState(null);
  const [amount, setAmount] = useState("");
  const [description, setDescription] = useState("");

  const [showConfirmation, setShowConfirmation] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [transferError, setTransferError] = useState("");
  const [transferResult, setTransferResult] = useState(null);

  const [recipientInput, setRecipientInput] = useState("");
  const [recipient, setRecipient] = useState(null);
  const [isSearching, setIsSearching] = useState(false);
  const [searchError, setSearchError] = useState("");

  useEffect(() => {
    const fetchWallet = async () => {
      try {
        const data = await getMyWallet(accessToken);
        setWallet(data);
      } catch (error) {
        console.error("Failed to fetch wallet:", error);
      }
    };

    if (accessToken) fetchWallet();
  }, [accessToken]);

  const handleSearchRecipient = async (event) => {
    event.preventDefault();

    console.log("Cari penerima: ", recipientInput);

    const keyword = recipientInput.trim();

    if (!keyword) {
      setSearchError("Masukkan email atau nomor wallet penerima.");
      setRecipient(null);
      return;
    }

    setIsSearching(true);
    setSearchError("");
    setRecipient(null);
    setTransferError("");

    try {
      const data = keyword.includes("@")
        ? await getRecipientByEmail(keyword, accessToken)
        : await getRecipientByWalletNumber(keyword, accessToken);

      if (data.status !== "ACTIVE") {
        setSearchError(
          "Wallet penerima tidak aktif dan tidak dapat menerima transfer.",
        );
        return;
      }

      if (wallet && data.walletId === wallet.id) {
        setSearchError(
          "Kamu tidak dapat melakukan transfer ke wallet sendiri.",
        );
        return;
      }

      setRecipient(data);
    } catch (error) {
      console.error("Recipient search failed:", error);
      setSearchError(
        error.message ||
          "Penerima tidak ditemukan. Periksa kembali informasi penerima.",
      );
    } finally {
      setIsSearching(false);
    }
  };

  const handleContinueTransfer = (event) => {
    event.preventDefault();

    const transferAmount = Number(amount);

    if (!wallet) {
      setTransferError("Informasi wallet tidak tersedia.");
      return;
    }

    if (wallet.status !== "ACTIVE") {
      setTransferError("Wallet kamu tidak aktif.");
      return;
    }

    if (!recipient) {
      setTransferError("Silakan pilih penerima terlebih dahulu.");
      return;
    }

    if (!Number.isFinite(transferAmount) || transferAmount < 10000) {
      setTransferError("Minimum transfer adalah Rp 10.000.");
      return;
    }

    if (transferAmount > wallet.balance) {
      setTransferError("Saldo kamu tidak mencukupi.");
      return;
    }

    if (description.length > 250) {
      setTransferError("Maksimal 250 karakter.");
      return;
    }

    setTransferError("");
    setShowConfirmation(true);
  };

  const handleConfirmTransfer = async () => {
    if (!wallet || !recipient || isSubmitting) return;

    setIsSubmitting(true);
    setTransferError("");

    try {
      const result = await createTransfer(
        {
          senderWalletId: wallet.id,
          receiverWalletId: recipient.walletId,
          amount: Number(amount),
          description: description.trim() || null,
        },
        accessToken,
      );

      setTransferResult(result);
      setShowConfirmation(false);

      try {
        const updatedWallet = await getMyWallet(accessToken);
        setWallet(updatedWallet);
      } catch (error) {
        console.error("Failed to refresh wallet:", error);
      }
    } catch (error) {
      setTransferError(error.message || "Transfer gagal.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const resetTransfer = () => {
    setTransferResult(null);
    setRecipient(null);
    setRecipientInput("");
    setAmount("");
    setDescription("");
    setTransferError("");
    setSearchError("");
    setShowConfirmation(false);
  };

  return (
    <AppLayout
      crumb="Transaksi"
      activeItem="Transfer"
      title="Transfer"
      subtitle="Kirim saldo dengan mudah ke pengguna lainnya."
    >
      {transferResult ? (
        <section className="bg-white col-span-2 rounded-xl w-full p-8 text-center">
          <div className="mx-auto h-14 rounded-full w-full bg-emerald-50 flex items-center justify-center">
            <span className="text-2xl text-emerald-600">✓</span>
          </div>

          <h2 className="text-lg font-bold text-primary-dark mt-5">
            Transfer berhasil!
          </h2>

          <p className="text-xs text-muted mt-2">
            Saldo berhasil dikirim kepada {recipient?.fullName}.
          </p>

          <p className="text-2xl font-bold text-primary-dark mt-6">
            {formatRupiah(transferResult.amount)}
          </p>

          <div className="border border-gray-100 rounded-lg p-4 mt-6 text-left">
            <div className="flex justify-between gap-4 text-xs">
              <span className="text-muted">Reference Number</span>
              <span className="font-semibold text-primary-dark text-right">
                {transferResult.referenceNo}
              </span>
            </div>

            <div className="flex justify-between gap-4 text-xs mt-4">
              <span className="text-muted">Status</span>
              <span className="font-semibold text-emerald-700">
                {transferResult.status}
              </span>
            </div>
          </div>

          <button
            type="button"
            onClick={resetTransfer}
            className="w-full bg-primary text-white rounded-lg py-3 text-xs font-semibold mt-6 hover:opacity-90 transition"
          >
            Transfer kembali
          </button>
        </section>
      ) : (
        <div className="grid grid-cols-3 gap-5">
          {/* Form Transfer */}
          <section className="bg-white col-span-2 rounded-xl p-6 md:p-7">
            <div className="mb-7">
              <h2 className="text-base font-semibold text-primary-dark">
                Buat transfer baru
              </h2>

              <p className="text-xs text-muted mt-1">
                Masukkan informasi penerima dan nominal transfer.
              </p>
            </div>

            <form onSubmit={handleContinueTransfer}>
              {/* Penerima */}
              <div className="mb-6">
                <label
                  htmlFor="recipient"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Penerima
                </label>

                <div className="flex gap-2">
                  <input
                    id="recipient"
                    type="text"
                    value={recipientInput}
                    onChange={(event) => {
                      setRecipientInput(event.target.value);
                      setRecipient(null);
                      setAmount("");
                      setDescription("");
                      setSearchError("");
                      setTransferError("");
                    }}
                    placeholder="Masukkan nomor wallet atau email penerima"
                    className="min-w-0 flex-1 border border-gray-200 rounded-lg px-4 py-3 text-sm outline-none focus:border-primary transition"
                  />

                  <button
                    type="button"
                    onClick={handleSearchRecipient}
                    disabled={isSearching || !recipientInput.trim()}
                    className="rounded-lg bg-primary text-white text-xs font-semibold px-4 py-3 hover:opacity-90 transition disabled:opacity-50"
                  >
                    {isSearching ? "Mencari..." : "Cari"}
                  </button>
                </div>

                {searchError && (
                  <p className="text-xs text-red-500 mt-2">{searchError}</p>
                )}

                {recipient && (
                  <div className="mt-4 border border-emerald-100 bg-emerald-50 rounded-lg p-4">
                    <p className="text-xs font-semibold text-emerald-700 mb-3">
                      Penerima ditemukan
                    </p>

                    <p className="text-sm font-semibold text-primary-dark">
                      {recipient.fullName}
                    </p>

                    <p className="text-xs text-muted mt-1">{recipient.email}</p>

                    <div className="flex justify-between items-center mt-3">
                      <span className="text-xs text-muted">
                        {recipient.walletNumber}
                      </span>

                      <span className="text-[10px] font-semibold text-emerald-700 uppercase">
                        {recipient.status}
                      </span>
                    </div>
                  </div>
                )}
              </div>

              {/* Nominal */}
              <div className="mb-6">
                <label
                  htmlFor="transfer-amount"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Nominal transfer
                </label>

                <div className="border border-gray-200 rounded-lg px-4 py-3 flex items-center">
                  <span className="text-xs text-muted mr-2">Rp</span>

                  <input
                    id="transfer-amount"
                    type="number"
                    min="10000"
                    placeholder="0"
                    value={amount}
                    onChange={(event) => {
                      setAmount(event.target.value);
                      setTransferError("");
                    }}
                    disabled={!recipient}
                    className="w-full outline-none text-sm font-semibold text-primary-dark disabled:bg-transparent"
                  />
                </div>

                <p className="text-[10px] text-muted mt-2">
                  Minimum transfer Rp 10.000.
                </p>
              </div>

              {/* Catatan */}
              <div className="mb-8">
                <label
                  htmlFor="transfer-note"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Catatan (opsional)
                </label>

                <textarea
                  id="transfer-note"
                  rows={3}
                  maxLength={250}
                  disabled={!recipient}
                  value={description}
                  onChange={(event) => setDescription(event.target.value)}
                  placeholder="Tambahkan catatan untuk penerima"
                  className="w-full resize-none border border-gray-200 rounded-lg px-4 py-3 text-sm outline-none focus:border-primary transition disabled:bg-gray-50"
                />
              </div>

              {transferError && (
                <p className="text-xs text-red-500 mb-4">{transferError}</p>
              )}

              <button
                type="submit"
                disabled={!recipient || !amount || isSubmitting}
                className="w-full rounded-lg bg-primary text-white text-xs font-semibold py-3 hover:opacity-90 transition disabled:opacity-50"
              >
                Lanjutkan transfer
              </button>
            </form>
          </section>

          {/* Ringkasan Akun */}
          <aside className="bg-white rounded-xl p-6 h-fit">
            <h2 className="text-sm font-semibold text-primary-dark">
              Ringkasan akun
            </h2>

            <div className="mt-5">
              <p className="text-xs text-muted mb-2">Saldo tersedia</p>

              <p className="text-xl font-bold text-primary-dark">
                {formatRupiah(wallet?.balance)}
              </p>
            </div>

            <div className="border-t border-gray-100 mt-5 pt-4 flex items-center justify-between">
              <span className="text-xs text-muted">Status akun</span>

              <span
                className={`rounded-full px-3 py-1 text-[10px] font-semibold uppercase ${
                  wallet?.status === "ACTIVE"
                    ? "bg-emerald-50 text-emerald-700"
                    : "bg-red-50 text-red-700"
                }`}
              >
                {wallet?.status ?? "Memuat..."}
              </span>
            </div>
          </aside>
        </div>
      )}

      {/* Modal Konfirmasi */}
      {showConfirmation && (
        <div className="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-base font-semibold text-primary-dark">
              Konfirmasi transfer
            </h2>

            <p className="text-xs text-muted mt-1">
              Periksa kembali informasi transfer sebelum melanjutkan.
            </p>

            <div className="border border-gray-100 rounded-lg p-4 mt-5 space-y-4">
              <div>
                <p className="text-xs text-muted">Penerima</p>
                <p className="text-sm font-semibold text-primary-dark mt-1">
                  {recipient?.fullName}
                </p>
                <p className="text-xs text-muted mt-1">
                  {recipient?.walletNumber}
                </p>
              </div>

              <div className="border-t border-gray-100 pt-3">
                <p className="text-xs text-muted">Nominal transfer</p>
                <p className="text-lg font-bold text-primary-dark mt-1">
                  {formatRupiah(Number(amount))}
                </p>
              </div>

              {description.trim() && (
                <div className="border-t border-gray-100 pt-3">
                  <p className="text-xs text-muted">Catatan</p>
                  <p className="text-sm text-primary-dark mt-1">
                    {description}
                  </p>
                </div>
              )}
            </div>

            {transferError && (
              <p className="text-xs text-red-500 mt-4">{transferError}</p>
            )}

            <div className="flex gap-3 mt-6">
              <button
                type="button"
                disabled={isSubmitting}
                onClick={() => {
                  setShowConfirmation(false);
                  setTransferError("");
                }}
                className="flex-1 border border-gray-200 rounded-lg py-3 text-xs font-semibold text-muted disabled:opacity-50"
              >
                Kembali
              </button>

              <button
                type="button"
                disabled={isSubmitting}
                onClick={handleConfirmTransfer}
                className="flex-1 bg-primary text-white rounded-lg py-3 text-xs font-semibold hover:opacity-90 disabled:opacity-50"
              >
                {isSubmitting ? "Memproses..." : "Konfirmasi"}
              </button>
            </div>
          </div>
        </div>
      )}
    </AppLayout>
  );
};

export default TransferPage;
