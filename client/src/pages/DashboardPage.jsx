import { useEffect, useState } from "react";

import { AppLayout } from "../components/AppLayout";
import WalletBalance from "../features/wallet/WalletBalance";
import QuickActions from "../features/wallet/QuickActions";
import RecentTransactions from "../features/transactions/RecentTransactions";

import { getMyWallet } from "../services/walletService";
import { getTransactionsByWalletId } from "../services/transactionService";
import { useAuth } from "../context/useAuth";

const DashboardPage = () => {
  // Sementara untuk pengujian, sebelum authentication dibuat.
  const { accessToken, user } = useAuth();

  const [wallet, setWallet] = useState(null);
  const [transactions, setTransactions] = useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isMounted = true;

    async function loadDashboard() {
      if (!accessToken) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError("");

        const walletData = await getMyWallet(accessToken);

        const transactionData = await getTransactionsByWalletId(
          walletData.id,
          accessToken,
        );

        if (!isMounted) return;

        setWallet(walletData);
        setTransactions(transactionData);
      } catch (err) {
        if (!isMounted) return;

        setError(err?.message || "Terjadi kesalahan saat memuat dashboard.");
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    loadDashboard();

    return () => {
      isMounted = false;
    };
  }, [accessToken]);

  return (
    <AppLayout
      user={user}
      title="Beranda"
      eyebrow="Personal Wallet"
      description={`Selamat datang kembali, ${user?.fullName || "Pengguna"}`}
    >
      {loading ? (
        <p className="text-muted text-xs">Memuat dashboard...</p>
      ) : error ? (
        <p className="error-message text-red-600 text-xs font-medium">
          {error}
        </p>
      ) : wallet ? (
        <>
          <div className="dashboard-overview">
            <WalletBalance wallet={wallet} />
            <QuickActions />
          </div>

          <RecentTransactions
            transactions={transactions}
            walletId={wallet.id}
          />
        </>
      ) : (
        <p className="text-muted text-xs">Data wallet tidak ditemukan.</p>
      )}
    </AppLayout>
  );
};

export default DashboardPage;
