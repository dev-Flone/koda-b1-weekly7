import { Link, useLocation, Navigate } from "react-router";

import { FocusLayout } from "../../components/AppLayout";
import { Card, Button, rupiah } from "../../components/ui";
import { SuccessIcon } from "../../components/SuccessIcon";

export default function TopupBerhasil() {
  const location = useLocation();

  const topUp = location.state?.topUp;
  const paymentMethod = location.state?.paymentMethod;

  if (!topUp || !paymentMethod) {
    return <Navigate to="/topup" replace />;
  }

  return (
    <FocusLayout>
      <Card className="!bg-slate-50 text-center shadow-none">
        <SuccessIcon />

        <h1 className="text-lg font-semibold text-slate-900">
          Isi saldo berhasil
        </h1>

        <p className="mt-1 text-xs text-slate-500">
          Saldo kamu bertambah {rupiah(topUp.amount)}.
        </p>

        <div className="mt-4 space-y-2 text-left">
          <div className="flex justify-between text-xs">
            <span className="text-slate-500">Metode pembayaran</span>
            <span className="font-medium text-slate-900">
              {paymentMethod.name}
            </span>
          </div>

          <div className="flex justify-between text-xs">
            <span className="text-slate-500">Nomor referensi</span>
            <span className="font-medium text-slate-900">
              {topUp.providerReference || topUp.id}
            </span>
          </div>

          <div className="flex justify-between text-xs">
            <span className="text-slate-500">Nominal</span>
            <span className="font-medium text-slate-900">
              {rupiah(topUp.amount)}
            </span>
          </div>
        </div>

        <Button as={Link} to="/" className="mt-5">
          Kembali ke beranda
        </Button>

        <Link
          to="/topup"
          className="mt-3 inline-block text-xs font-medium text-teal-700 hover:underline"
        >
          Isi saldo lagi
        </Link>
      </Card>
    </FocusLayout>
  );
}
