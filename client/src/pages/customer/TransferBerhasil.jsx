import { Link } from "react-router";
import { FocusLayout } from "../../components/AppLayout";
import { Card, Button } from "../../components/ui";
import { SuccessIcon } from "../../components/SuccessIcon";

export default function TransferBerhasil() {
  return (
    <FocusLayout>
      <Card className="!bg-slate-50 text-center shadow-none">
        <SuccessIcon />
        <h1 className="text-lg font-semibold text-slate-900">
          Transfer berhasil
        </h1>
        <p className="mt-1 text-xs text-slate-500">
          Rp 150.000 telah dikirim ke Budi Santoso.
        </p>
        <p className="mt-3 text-xs text-slate-400">No. referensi TRX-20917</p>
        <Button as={Link} to="/dashboard" className="mt-5">
          Kembali ke beranda
        </Button>
        <Link
          to="/aktivitas"
          className="mt-3 inline-block text-xs font-medium text-teal-700 hover:underline"
        >
          Lihat aktivitas
        </Link>
      </Card>
    </FocusLayout>
  );
}
