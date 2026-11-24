import { useState } from "react";
import { useLocation, Navigate, useNavigate } from "react-router";

import { FocusLayout } from "../../components/AppLayout";
import { Card, Row, Button, rupiah } from "../../components/ui";
import { confirmTopUp } from "../../services/topUpService";

// Placeholder QR (bukan QRIS pembayaran sungguhan).
function FakeQr() {
  const n = 21;

  const cells = Array.from({ length: n * n }, (_, i) => {
    const x = i % n;
    const y = Math.floor(i / n);

    const finder = (x < 7 && y < 7) || (x > 13 && y < 7) || (x < 7 && y > 13);

    if (finder) {
      const fx = x % 14 > 6 ? x - 14 : x;
      const fy = y > 13 ? y - 14 : y;

      return (
        fx === 0 ||
        fx === 6 ||
        fy === 0 ||
        fy === 6 ||
        (fx > 1 && fx < 5 && fy > 1 && fy < 5)
      );
    }

    return (x * 7 + y * 13 + x * y) % 3 === 0;
  });

  return (
    <div
      className="grid aspect-square w-40 gap-0"
      style={{ gridTemplateColumns: `repeat(${n},1fr)` }}
    >
      {cells.map((on, i) => (
        <div key={i} className={on ? "bg-slate-900" : "bg-white"} />
      ))}
    </div>
  );
}

export default function TopupQris() {
  const location = useLocation();
  const navigate = useNavigate();

  const topUp = location.state?.topUp;
  const paymentMethod = location.state?.paymentMethod;

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  if (!topUp || !paymentMethod) {
    return <Navigate to="/topup" replace />;
  }

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
      setError(err.message || "Konfirmasi pembayaran gagal.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <FocusLayout back={{ to: "/topup/konfirmasi", label: "Kembali" }}>
      <Card className="bg-slate-50! shadow-none">
        <h1 className="text-lg font-semibold text-slate-900">
          Pembayaran QRIS
        </h1>

        <p className="mb-4 text-xs text-slate-500">
          Scan kode QR di bawah sebelum waktu habis.
        </p>

        <div className="mx-auto mb-4 w-fit rounded-xl bg-white p-3 ring-1 ring-slate-100">
          <FakeQr />
        </div>

        <p className="mb-3 text-center text-xs text-slate-500">
          Status pembayaran:{" "}
          <span className="font-medium text-amber-600">{topUp.status}</span>
        </p>

        <div className="divide-y divide-slate-100">
          <Row label="Order ID" value={String(topUp.id)} />
          <Row label="Metode" value={paymentMethod.name} />
          <Row label="Status" value={topUp.status} />
        </div>

        <div className="mt-2 border-t border-slate-200 pt-2">
          <Row label="Total pembayaran" value={rupiah(topUp.amount)} strong />
        </div>

        {error && <p className="mt-3 text-xs text-red-600">{error}</p>}

        <Button
          onClick={handleConfirmPayment}
          disabled={isSubmitting}
          className="mt-5"
        >
          {isSubmitting ? "Memproses pembayaran..." : "Saya sudah membayar"}
        </Button>
      </Card>
    </FocusLayout>
  );
}
