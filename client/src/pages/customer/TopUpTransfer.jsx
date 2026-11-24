import { useState } from "react";
import { Navigate, useLocation, useNavigate } from "react-router";
import { confirmTopUp } from "../../services/topUpService";
import { FocusLayout } from "../../components/AppLayout";
import { Button, Card, Row, rupiah } from "../../components/ui";

const TopUpTransfer = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const topUp = location?.state?.topUp;
  const paymentMethod = location?.state?.paymentMethod;

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  if (!topUp || !paymentMethod) return <Navigate to="/topup" replace />;

  const handleConfirmPayment = async () => {
    try {
      setIsSubmitting(true);
      setError("");

      const confirmedTopUp = await confirmTopUp(topUp.id);

      navigate("/topup/berhasil", {
        replace: true,
        state: {
          topUp: confirmedTopUp,
          paymentMethod,
        },
      });
    } catch (err) {
      setError(err.message || "Konfirmasi transfer gagal.");
    } finally {
      setIsSubmitting(false);
    }
  };
  return (
    <FocusLayout back={{ to: "/topup/konfirmasi", label: "kembali" }}>
      <Card className="bg-slate-50! shadow-none">
        <h1 className="text-lg font-semibold text-slate-900">
          Pembayaran transfer sesuai informasi pembayaran berikut.
        </h1>

        <p className="mb-4 text-xs text-slate-500">
          Silakan transfer sesuai informasi pembayaran berikut.
        </p>

        <div className="rounded-xl border border-slate-200 bg-white p-4">
          <p className=" text-xs text-slate-500">Bank Tujuan</p>

          <p className="mt-1 text-sm font-semibold text-slate-900">
            {paymentMethod.providerName}
          </p>

          <div className="mt-4">
            <p className="text-xs text-slate-500">Nomor rekening</p>

            <p className="mt-1 text-lg font-semibold tracking-wider text-slate-900">
              Belum tersedia
            </p>
          </div>

          <div className="mt-4">
            <p className="text-xs text-slate-500">Atas nama</p>

            <p className="mt-1 text-lg font-medium text-slate-900">
              Belum tersedia
            </p>
          </div>
        </div>

        <div className="mt-5 divide-y divide-slate-100">
          <Row label="Order ID" value={String(topUp.id)} />
          <Row label="Metode" value={paymentMethod.name} />
          <Row label="Status" value={topUp.status} />
        </div>

        <div className="mt-2 border-t border-slate-200 pt-2">
          <Row label="Total transfer" value={rupiah(topUp.amount)} strong />
        </div>

        <div className="mt-4 rounded-lg bg-amber-50 p-3">
          <p className="text-cs font-semibold text-amber-800">
            Instruksi pembayaran
          </p>

          <ol className="mt-2 list-decimal space-y-1 pl-4 text-xs leading-relaxed text-amber-700">
            <li>Buka aplikasi mobile banking kamu.</li>
            <li>Pilih menu transfer antarbank atau sesama bank.</li>
            <li>Masukkan rekening tujuan yang telah diverifikasi.</li>
            <li>Transfer sesuai dengan nominal yang tertera.</li>
            <li>
              Pastikan pembayaran berhasil sebelum meninggalkan halaman ini.
            </li>
          </ol>
        </div>

        {error && <p className="mt-3 text-xs text-red-600">{error}</p>}

        <Button
          onClick={handleConfirmPayment}
          disabled={isSubmitting}
          className="mt-5"
        >
          {isSubmitting ? "Memproses pembayaran..." : "Saya sudah transfer"}
        </Button>
      </Card>
    </FocusLayout>
  );
};

export default TopUpTransfer;
