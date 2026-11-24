import { Link } from "react-router";
import { FocusLayout } from "../../components/AppLayout";
import { Card, Row, Button, rupiah } from "../../components/ui";

export default function TransferKonfirmasi() {
  return (
    <FocusLayout back={{ to: "/transfer", label: "Ubah detail" }}>
      <Card className="!bg-slate-50 shadow-none">
        <h1 className="text-lg font-semibold text-slate-900">
          Konfirmasi transfer
        </h1>
        <p className="mb-4 text-xs text-slate-500">
          Periksa kembali detail sebelum mengirim.
        </p>
        <div className="divide-y divide-slate-100">
          <Row label="Penerima" value="Budi Santoso" />
          <Row label="Nominal" value={rupiah(150000)} />
          <Row label="Catatan" value="Patungan makan siang" />
        </div>
        <div className="mt-2 border-t border-slate-200 pt-2">
          <Row label="Total dikirim" value={rupiah(150000)} strong />
        </div>
        <Button as={Link} to="/transfer/berhasil" className="mt-5">
          Kirim sekarang
        </Button>
      </Card>
    </FocusLayout>
  );
}
