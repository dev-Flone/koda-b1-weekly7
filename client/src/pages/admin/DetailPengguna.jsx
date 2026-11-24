import { Link } from "react-router";
import { AppLayout } from "../../components/AppLayout";
import {
  Card,
  CardTitle,
  Avatar,
  Badge,
  Button,
  Row,
  rupiah,
} from "../../components/ui";

export default function DetailPengguna() {
  return (
    <AppLayout
      role="admin"
      crumb="Admin / Pengguna"
      title="Detail pengguna"
      subtitle="Informasi lengkap dan tindakan akun."
    >
      <div className="grid gap-5 lg:grid-cols-[1fr_280px]">
        <Card>
          <Link
            to="/admin/pengguna"
            className="mb-4 inline-block text-xs font-medium text-teal-700 hover:underline"
          >
            ← Kembali ke daftar
          </Link>
          <div className="mb-4 flex items-center gap-3">
            <Avatar name="Rani Putri" size="h-12 w-12" />
            <div className="flex-1">
              <p className="font-semibold text-slate-900">Rani Putri</p>
              <p className="text-xs text-slate-500">rani@mail.com</p>
            </div>
            <Badge>Aktif</Badge>
          </div>
          <div className="divide-y divide-slate-100">
            <Row label="ID pengguna" value="USR-00123" />
            <Row label="Telepon" value="0812 3456 7890" />
            <Row label="Saldo" value={rupiah(1250000)} />
            <Row label="Terdaftar" value="12 Agu 2026" />
            <Row label="Transaksi terakhir" value="24 Nov 2026" />
          </div>
        </Card>

        <Card className="h-fit">
          <CardTitle title="Status akun" />
          <Badge>Aktif</Badge>
          <p className="my-3 text-xs text-slate-500">
            Memblokir akun akan menghentikan seluruh transaksi pengguna.
          </p>
          <Button variant="danger">Blokir pengguna</Button>
        </Card>
      </div>
    </AppLayout>
  );
}
