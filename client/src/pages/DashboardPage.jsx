import { useEffect, useState } from "react";

import Sidebar from "../components/layout/Sidebar";
import Header from "../components/layout/Header";
import WalletBalance from "../features/wallet/WalletBalance";
import QuickActions from "../features/wallet/QuickActions";
import RecentTransactions from "../features/transactions/RecentTransactions";

import { getWalletById } from "../services/walletService";
import { getTransactionsByWalletId } from "../services/transactionService";

const DashboardPage = () => {
  // Sementara untuk pengujian, sebelum authentication dibuat.
  const walletId = 21;

  const [wallet, setWallet] = useState(null);
  const [transactions, setTransactions] = useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isMounted = true;

    async function loadDashboard() {
      try {
        setLoading(true);
        setError("");

        const [walletData, transactionData] = await Promise.all([
          getWalletById(walletId),
          getTransactionsByWalletId(walletId),
        ]);

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
  }, [walletId]);

  return (
    <div className="app-layout">
      <Sidebar />

      <main className="main-content">
        <Header />

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

            <RecentTransactions transactions={transactions} />
          </>
        ) : (
          <p className="text-muted text-xs">Data wallet tidak ditemukan.</p>
        )}
      </main>
    </div>
  );
};

export default DashboardPage;
