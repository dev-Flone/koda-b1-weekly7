import { Link } from "react-router";
import { AppLayout } from "../../components/AppLayout";
import {
  Card,
  CardTitle,
  Field,
  Input,
  Button,
  Avatar,
  Row,
  rupiah,
} from "../../components/ui";

export default function Transfer() {
  return (
    <AppLayout
      crumb="Transfer"
      title="Transfer saldo"
      subtitle="Kirim saldo ke pengguna lain secara instan."
    >
      <div className="grid gap-5 lg:grid-cols-[1fr_300px]">
        <Card>
          <CardTitle
            title="Detail transfer"
            desc="Isi tujuan dan nominal yang akan dikirim."
          />
          <form className="space-y-5">
            <Field label="Email / nomor penerima">
              <Input
                placeholder="penerima@mail.com"
                defaultValue="budi@mail.com"
              />
            </Field>

            {/* Kartu penerima hasil pencarian */}
            <div className="flex items-center gap-3 rounded-xl bg-teal-50 p-3">
              <Avatar name="Budi Santoso" />
              <div className="flex-1">
                <p className="text-sm font-medium text-slate-900">
                  Budi Santoso
                </p>
                <p className="text-xs text-slate-500">
                  budi@mail.com · Terverifikasi
                </p>
              </div>
              <button
                type="button"
                className="text-xs text-teal-700 hover:underline"
              >
                Ubah
              </button>
            </div>

            <Field label="Nominal">
              <Input placeholder="Rp 0" defaultValue="Rp 150.000" />
            </Field>
            <Field label="Catatan (opsional)">
              <Input placeholder="Contoh: patungan makan siang" />
            </Field>

            <Button as={Link} to="/transfer/konfirmasi">
              Lanjutkan
            </Button>
          </form>
        </Card>

        <Card className="h-fit">
          <CardTitle title="Saldo tersedia" />
          <p className="text-2xl font-semibold text-slate-900">
            {rupiah(1250000)}
          </p>
          <div className="mt-4 border-t border-slate-100 pt-3">
            <Row label="Biaya transfer" value="Gratis" />
            <Row label="Batas harian" value={rupiah(10000000)} />
          </div>
        </Card>
      </div>
    </AppLayout>
  );
}
