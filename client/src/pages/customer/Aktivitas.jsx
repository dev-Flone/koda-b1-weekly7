import { useEffect, useMemo, useState } from "react";

import { getMyWallet } from "../../services/walletService";
import { getTransactionsByWalletId } from "../../services/transactionService";
import { useAuth } from "../../context/useAuth";

import { AppLayout } from "../../components/AppLayout";
import {
  Card,
  CardTitle,
  Badge,
  Table,
  Td,
  inputCls,
  Button,
  rupiah,
} from "../../components/ui";
import {
  getActivePaymentMethods,
  getTopUpsByWalletId,
} from "../../services/topUpService";
import { useNavigate } from "react-router";

const tone = {
  Berhasil: "success",
  Menunggu: "warning",
  Gagal: "danger",
  Kadaluarsa: "danger",
};

const PAGE_SIZE = 5;

const parseUtcDate = (date) => {
  if (!date) return null;

  const utcDate = /Z$|[+-]\d{2}:\d{2}$/.test(date) ? date : `${date}Z`;

  return new Date(utcDate);
};

const formatTransactionDate = (date) => {
  return new Intl.DateTimeFormat("id-ID", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    timeZone: "Asia/Jakarta",
  }).format(parseUtcDate(date));
};

const getTransactionTitle = (transaction) => {
  if (transaction.description) {
    return transaction.description;
  }

  switch (transaction.type) {
    case "TOPUP":
      return "Isi Saldo";
    case "TRANSFER":
      return "Transfer";
    default:
      return transaction.type;
  }
};

const getTransactionStatus = (status) => {
  switch (status) {
    case "SUCCESS":
    case "COMPLETED":
      return "Berhasil";
    case "PENDING":
      return "Menunggu";
    case "FAILED":
      return "Gagal";
    default:
      return status;
  }
};

const isTopUpExpired = (topUp) => {
  if (!topUp) return false;

  if (topUp.status === "EXPIRED") return true;

  return (
    topUp.status === "PENDING" &&
    topUp.expiresAtUtc &&
    parseUtcDate(topUp.expiresAtUtc) <= new Date()
  );
};

export default function Aktivitas() {
  const navigate = useNavigate();

  const { accessToken } = useAuth();

  const [paymentError, setPaymentError] = useState("");

  // Search dan filter
  const [searchTerm, setSearchTerm] = useState("");
  const [typeFilter, setTypeFilter] = useState("ALL");
  const [statusFilter, setStatusFilter] = useState("ALL");
  const [periodFilter, setPeriodFilter] = useState("ALL");
  const [sortOrder, setSortOrder] = useState("NEWEST");

  // Load More
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE);

  // API data
  const [wallet, setWallet] = useState(null);
  const [transactions, setTransactions] = useState([]);
  const [topUps, setTopUps] = useState([]);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleContinuePayment = async (topUp) => {
    try {
      setPaymentError("");

      const paymentMethods = await getActivePaymentMethods();

      const paymentMethod = paymentMethods.find(
        (method) => method.id === topUp.paymentMethodId,
      );

      if (!paymentMethod) throw new Error("Metode pembayaran tidak tersedia");

      const paymentRoute =
        paymentMethod.code === "QRIS" ? "/topup/qris" : "/topup/transfer";

      navigate(paymentRoute, {
        state: {
          topUp,
          paymentMethod,
        },
      });
    } catch (error) {
      setPaymentError(error.message || "Gagal melanjutkan pembayaran.");
    }
  };

  // Mengambil wallet dan riwayat transaksi
  useEffect(() => {
    let isMounted = true;
    let isFetching = false;
    let walletId = null;

    async function loadData() {
      if (!accessToken || isFetching) return;

      isFetching = true;
      setLoading(true);

      try {
        // Ambil wallet hanya pada pemuatan pertama
        if (!walletId) {
          const walletData = await getMyWallet(accessToken);

          if (!isMounted) return;

          walletId = walletData.id;
          setWallet(walletData);
        }

        // Refresh transaksi dan top-up
        const [transactionData, topUpData] = await Promise.all([
          getTransactionsByWalletId(walletId, accessToken),
          getTopUpsByWalletId(walletId),
        ]);

        if (isMounted) {
          setTransactions(transactionData);
          setTopUps(topUpData);
          setError("");
        }
      } catch (err) {
        if (isMounted) {
          setError(err.message || "Gagal memuat aktivitas.");
        }
      } finally {
        isFetching = false;

        if (isMounted) {
          setLoading(false);
        }
      }
    }

    if (!accessToken) return;

    // Pemanggilan pertama
    loadData();

    // Refresh setiap 30 detik
    const intervalId = setInterval(loadData, 30_000);

    return () => {
      isMounted = false;
      clearInterval(intervalId);
    };
  }, [accessToken]);

  // Filtering dan sorting
  const filteredTransactions = useMemo(() => {
    let result = [...transactions];

    // Search
    if (searchTerm.trim()) {
      const keyword = searchTerm.toLowerCase().trim();

      result = result.filter((transaction) => {
        return (
          transaction.description?.toLowerCase().includes(keyword) ||
          transaction.referenceNo?.toLowerCase().includes(keyword)
        );
      });
    }

    // Filter jenis transaksi
    if (typeFilter !== "ALL") {
      result = result.filter((transaction) => transaction.type === typeFilter);
    }

    // Filter status
    if (statusFilter !== "ALL") {
      result = result.filter((transaction) => {
        const topUp = topUps.find(
          (item) => item.transactionId === transaction.id,
        );

        if (statusFilter === "EXPIRED") {
          return isTopUpExpired(topUp);
        }

        if (statusFilter === "FAILED") {
          return transaction.status === "FAILED" && !isTopUpExpired(topUp);
        }

        return transaction.status === statusFilter;
      });
    }

    // Filter periode
    if (periodFilter !== "ALL") {
      const now = new Date();
      const days = Number(periodFilter);
      const startDate = new Date(now);

      startDate.setDate(now.getDate() - days);

      result = result.filter((transaction) => {
        const transactionDate = parseUtcDate(transaction.createdAtUtc);

        return (
          transactionDate &&
          !Number.isNaN(transactionDate.getTime()) &&
          transactionDate >= startDate &&
          transactionDate <= now
        );
      });
    }

    // Sorting
    result.sort((a, b) => {
      const dateA = parseUtcDate(a.createdAtUtc)?.getTime() ?? 0;
      const dateB = parseUtcDate(b.createdAtUtc)?.getTime() ?? 0;

      return sortOrder === "NEWEST" ? dateB - dateA : dateA - dateB;
    });

    return result;
  }, [
    transactions,
    topUps,
    searchTerm,
    typeFilter,
    statusFilter,
    periodFilter,
    sortOrder,
  ]);

  // Data yang ditampilkan
  const displayedTransactions = filteredTransactions.slice(0, visibleCount);

  const hasMore = visibleCount < filteredTransactions.length;

  return (
    <AppLayout
      crumb="Aktivitas"
      activeItem="Aktivitas"
      title="Aktivitas"
      subtitle="Semua riwayat isi saldo dan transfer kamu."
    >
      <Card>
        <CardTitle title="Riwayat transaksi" />

        {/* Search dan filter */}
        <div className="mb-4 grid gap-3 sm:grid-cols-[1fr_150px_150px_150px]">
          <input
            type="text"
            className={inputCls}
            placeholder="Cari transaksi..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setVisibleCount(PAGE_SIZE);
            }}
          />

          <select
            className={inputCls}
            value={typeFilter}
            onChange={(e) => {
              setTypeFilter(e.target.value);
              setVisibleCount(PAGE_SIZE);
            }}
          >
            <option value="ALL">Semua jenis</option>
            <option value="TOPUP">Isi saldo</option>
            <option value="TRANSFER">Transfer</option>
          </select>

          <select
            className={inputCls}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setVisibleCount(PAGE_SIZE);
            }}
          >
            <option value="ALL">Semua status</option>
            <option value="SUCCESS">Berhasil</option>
            <option value="PENDING">Menunggu</option>
            <option value="FAILED">Gagal</option>
            <option value="EXPIRED">Kadaluarsa</option>
          </select>

          <select
            className={inputCls}
            value={sortOrder}
            onChange={(e) => {
              setSortOrder(e.target.value);
              setVisibleCount(PAGE_SIZE);
            }}
          >
            <option value="NEWEST">Terbaru</option>
            <option value="OLDEST">Terlama</option>
          </select>
        </div>

        {/* Filter periode */}
        <div className="mb-4 flex flex-wrap items-center gap-2">
          <span className="text-xs text-slate-500">Periode:</span>

          {[
            { label: "Semua waktu", value: "ALL" },
            { label: "7 hari", value: "7" },
            { label: "30 hari", value: "30" },
            { label: "90 hari", value: "90" },
          ].map((period) => (
            <button
              key={period.value}
              type="button"
              onClick={() => {
                setPeriodFilter(period.value);
                setVisibleCount(PAGE_SIZE);
              }}
              className={`rounded-lg border px-3 py-1.5 text-xs transition ${
                periodFilter === period.value
                  ? "border-slate-900 bg-slate-900 text-white"
                  : "border-slate-200 text-slate-600 hover:bg-slate-50"
              }`}
            >
              {period.label}
            </button>
          ))}
        </div>

        {paymentError && (
          <p className="mb-3 text-xs text-red-600">{paymentError}</p>
        )}

        {/* Tabel transaksi */}
        <Table head={["Transaksi", "Tanggal", "Nominal", "Status"]}>
          {loading ? (
            <tr>
              <Td className="text-center" colSpan={4}>
                Memuat riwayat transaksi...
              </Td>
            </tr>
          ) : error ? (
            <tr>
              <Td className="text-center text-red-600" colSpan={4}>
                {error}
              </Td>
            </tr>
          ) : filteredTransactions.length === 0 ? (
            <tr>
              <Td className="text-center" colSpan={4}>
                {transactions.length === 0
                  ? "Belum ada transaksi"
                  : "Tidak ada transaksi yang sesuai dengan filter"}
              </Td>
            </tr>
          ) : (
            displayedTransactions.map((transaction) => {
              const isOutgoing = transaction.senderWalletId === wallet?.id;

              const topUp = topUps.find(
                (item) => item.transactionId === transaction.id,
              );

              const isPendingTopUp = topUp?.status === "PENDING";

              const isExpired = isTopUpExpired(topUp);

              const canContinuePayment =
                transaction.type === "TOPUP" &&
                isPendingTopUp &&
                topUp.expiresAtUtc &&
                !isExpired;

              const statusLabel = isExpired
                ? "Kadaluarsa"
                : getTransactionStatus(transaction.status);

              return (
                <tr key={transaction.id}>
                  <Td>
                    <p className="font-medium text-slate-900">
                      {getTransactionTitle(transaction)}
                    </p>

                    <p className="text-xs text-slate-400">
                      {transaction.referenceNo}
                    </p>

                    {canContinuePayment && (
                      <button
                        type="button"
                        onClick={() => handleContinuePayment(topUp)}
                        className="mt-2 text-xs font-medium text-teal-700 hover:underline"
                      >
                        Lanjutkan pembayaran
                      </button>
                    )}
                  </Td>

                  <Td className="text-slate-500">
                    {formatTransactionDate(transaction.createdAtUtc)}
                  </Td>

                  <Td
                    className={`font-medium ${
                      isOutgoing ? "text-slate-900" : "text-emerald-700"
                    }`}
                  >
                    {isOutgoing ? "-" : "+"} {rupiah(transaction.amount)}
                  </Td>

                  <Td>
                    <Badge tone={tone[statusLabel] || "warning"}>
                      {statusLabel}
                    </Badge>
                  </Td>
                </tr>
              );
            })
          )}
        </Table>

        {/* Load More */}
        {!loading && !error && filteredTransactions.length > 0 && (
          <div className="mt-4 flex flex-col items-center gap-3">
            <span className="text-xs text-slate-500">
              Menampilkan {displayedTransactions.length} dari{" "}
              {filteredTransactions.length} transaksi
            </span>

            {hasMore && (
              <Button
                variant="soft"
                className="w-auto! py-1.5! text-xs"
                onClick={() =>
                  setVisibleCount((prev) =>
                    Math.min(prev + PAGE_SIZE, filteredTransactions.length),
                  )
                }
              >
                Muat lebih banyak
              </Button>
            )}
          </div>
        )}
      </Card>
    </AppLayout>
  );
}
