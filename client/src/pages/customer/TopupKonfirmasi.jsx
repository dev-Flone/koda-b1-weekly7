import { Link, Navigate, useLocation } from "react-router";
import { FocusLayout } from "../../components/AppLayout";
import { Card, Row, Button, rupiah } from "../../components/ui";

export default function TopupKonfirmasi() {
  const location = useLocation();

  const topUp = location.state?.topUp;
  const paymentMethod = location.state?.paymentMethod;

  if (!topUp || !paymentMethod) return <Navigate to="/topup" replace />;
  return (
    <FocusLayout back={{ to: "/topup", label: "Ubah detail" }}>
      <Card className="bg-slate-50! shadow-none">
        <h1 className="text-lg font-semibold text-slate-900">
          Konfirmasi isi saldo
        </h1>
        <p className="mb-4 text-xs text-slate-500">
          Pastikan nominal dan metode sudah benar.
        </p>

        <div className="divide-y divide-slate-100">
          <Row label="Metode" value={paymentMethod.name} />
          <Row label="Nominal" value={rupiah(topUp.amount)} />
          <Row label="Biaya admin" value="Gratis" />
        </div>
        <div className="mt-2 border-t border-slate-200 pt-2">
          <Row label="Total pembayaran" value={rupiah(topUp.amount)} strong />
        </div>
        <Button
          as={Link}
          to={paymentMethod.code === "QRIS" ? "/topup/qris" : "/topup/transfer"}
          state={{ topUp, paymentMethod }}
          className="mt-5"
        >
          Lanjut ke pembayaran
        </Button>
      </Card>
    </FocusLayout>
  );
}
