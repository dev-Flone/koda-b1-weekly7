import { useEffect, useState } from "react";
import { useAuth } from "../context/useAuth";
import { getMyWallet } from "../services/walletService";
import { createTopUp, getActivePaymentMethods } from "../services/topUpService";
import { useNavigate } from "react-router";
import { AppLayout } from "../components/AppLayout";

const formatCurrency = (amount) => {
  return new Intl.NumberFormat("id-ID", {
    style: "currency",
    currency: "IDR",
    maximumFractionDigits: 0,
  }).format(amount || 0);
};

const TopUpPage = () => {
  const { accessToken, user } = useAuth();
  const navigate = useNavigate();

  const [wallet, setWallet] = useState(null);
  const [amount, setAmount] = useState("");

  const [paymentMethods, setPaymentMethods] = useState([]);
  const [paymentMethodId, setPaymentMethodId] = useState("");

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [createdTopUp, setCreatedTopUp] = useState(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // LOAD WALLET
  useEffect(() => {
    let isMounted = true;

    async function loadWallet() {
      if (!accessToken) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError("");

        const [walletData, methodsData] = await Promise.all([
          getMyWallet(accessToken),
          getActivePaymentMethods(),
        ]);

        if (isMounted) {
          setWallet(walletData);
          setPaymentMethods(methodsData);

          if (methodsData.length > 0) {
            setPaymentMethodId(String(methodsData[0].id));
          }
        }
      } catch (err) {
        if (isMounted) {
          setError(err?.message || "Gagal memuat data top up");
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }
    loadWallet();

    return () => {
      isMounted = false;
    };
  }, [accessToken]);

  // HANDLE SUBMIT
  const handleSubmit = async (e) => {
    e.preventDefault();

    const numericAmount = Number(amount);

    if (!wallet?.id) {
      setError("Informasi wallet tidak ditemukan");
      return;
    }

    if (!numericAmount || numericAmount < 10000) {
      setError("Minimal top up adalah Rp 10.000");
      return;
    }

    if (paymentMethodId === "") {
      setError("Silahkan pilih methode pembayaran");
      return;
    }

    try {
      setIsSubmitting(true);
      setError("");
      setCreatedTopUp(null);

      const response = await createTopUp({
        walletId: wallet.id,
        paymentMethodId: Number(paymentMethodId),
        amount: numericAmount,
      });

      const selectedMethod = paymentMethods.find(
        (method) => method.id === Number(paymentMethodId),
      );

      navigate("/topup/konfirmasi", {
        state: {
          topUp: response,
          paymentMethod: selectedMethod,
        },
      });
    } catch (error) {
      setError(error?.message || "Gagak nembuat TopUp request");
    } finally {
      setIsSubmitting(false);
    }
  };
  return (
    <AppLayout
      user={user}
      eyebrow="Transaksi"
      title="Isi Saldo"
      description="Pilih nominal dan metode pembayaran yang tersedia"
    >
      <div className="grid grid-cols-3 gap-5">
        {/* Form Top Up */}
        <section className="bg-white col-span-2 rounded-xl p-6 md:p-7">
          <div className="mb-7">
            <h2 className="text-base font-semibold text-primary-dark">
              Buat permintaan isi saldo
            </h2>

            <p className="text-xs text-muted mt-1">
              Saldo baru bertambah setelah pembayaran dikonfirmasi.
            </p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="mb-6">
              <label
                htmlFor="topup-amount"
                className="block text-xs font-medium text-muted mb-2"
              >
                Nominal isi saldo
              </label>

              <div className="flex border border-gray-200 rounded-lg px-4 py-3">
                <span className="text-xs text-muted mr-2">Rp</span>

                <input
                  id="topup-amount"
                  type="number"
                  min="10000"
                  step="1000"
                  value={amount}
                  onChange={(event) => setAmount(event.target.value)}
                  placeholder="Masukkan nominal"
                  className="w-full outline-none text-sm font-semibold text-primary-dark"
                />
              </div>
            </div>

            <div className="mb-8">
              <p className="text-xs font-medium text-muted mb-3">
                Metode pembayaran
              </p>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {paymentMethods.map((method) => (
                  <button
                    key={method.id}
                    type="button"
                    onClick={() => setPaymentMethodId(String(method.id))}
                    className={`text-left rounded-lg border p-4 transition ${
                      paymentMethodId === String(method.id)
                        ? "border-primary bg-primary/5"
                        : "border-gray-200 hover:border-primary/40"
                    }`}
                  >
                    <span className="block text-sm font-semibold text-primary-dark">
                      {method.name}
                    </span>

                    <span className="block text-xs text-muted mt-1">
                      {method.providerName || method.code}
                    </span>
                  </button>
                ))}
              </div>
            </div>

            {error && <p className="text-xs text-red-600 mb-4">{error}</p>}

            <button
              type="submit"
              disabled={
                isSubmitting ||
                loading ||
                !wallet ||
                !amount ||
                Number(amount) < 10000 ||
                !paymentMethodId
              }
              className="w-full rounded-lg bg-primary text-white text-xs font-semibold py-3 hover:opacity-90 disabled:opacity-50 transition"
            >
              {isSubmitting ? "Memproses..." : "Lanjutkan pembayaran"}
            </button>

            {error && <p className="text-xs text-red-600 mb-4">{error}</p>}
            {error && <p className="text-xs text-red-600 mb-4">{error}</p>}

            {createdTopUp && (
              <div className="mt-5 rounded-lg border border-emerald-200 bg-emerald-50 p-4">
                <div className="flex items-center justify-between mb-3">
                  <h3 className="text-sm font-semibold text-emerald-800">
                    Top Up berhasil dibuat
                  </h3>

                  <span className="text-[10px] font-semibold text-amber-700 bg-amber-100 px-2 py-1 rounded-full">
                    {createdTopUp.status}
                  </span>
                </div>

                <p className="text-xs text-muted mb-1">Nominal Top Up</p>

                <p className="text-xl font-bold text-primary-dark">
                  {formatCurrency(createdTopUp.amount)}
                </p>

                <div className="border-t border-emerald-200 mt-4 pt-3 space-y-2">
                  <div className="flex justify-between text-xs">
                    <span className="text-muted">Top Up ID</span>
                    <span className="font-medium">{createdTopUp.id}</span>
                  </div>

                  <div className="flex justify-between text-xs">
                    <span className="text-muted">Transaction ID</span>
                    <span className="font-medium">
                      {createdTopUp.transactionId}
                    </span>
                  </div>

                  <div className="flex justify-between text-xs">
                    <span className="text-muted">Metode pembayaran</span>
                    <span className="font-medium">
                      {
                        paymentMethods.find(
                          (method) =>
                            method.id === createdTopUp.paymentMethodId,
                        )?.name
                      }
                    </span>
                  </div>
                </div>
              </div>
            )}
          </form>
        </section>

        {/* Ringkasan Akun */}

        <aside className="bg-white rounded-xl p-6 h-fit">
          <h2 className="text-sm font-semibold text-primary-dark">
            Ringkasan akun
          </h2>

          <div className="mt-5">
            <p className="text-xs text-muted mb-2">Saldo saat ini</p>

            {loading ? (
              <p className="text-xs text-muted">Memuat saldo...</p>
            ) : (
              <p className="text-xl font-bold text-primary-dark">
                {formatCurrency(wallet?.balance)}
              </p>
            )}
          </div>

          <div className="border-t border-gray-100 mt-5 pt-4 flex items-center justify-between">
            <span className="text-xs text-muted">Status akun</span>

            <span className="rounded-full bg-emerald-50 text-emerald-700 px-3 py-1 text-[10px] font-semibold uppercase">
              {wallet?.status || "Aktif"}
            </span>
          </div>
        </aside>
      </div>
    </AppLayout>
  );
};

export default TopUpPage;
