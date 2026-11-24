import { Link } from "react-router";
import { AppLayout } from "../../components/AppLayout";
import { Card, CardTitle, Badge, Table, Td, rupiah } from "../../components/ui";

const stats = [
  { label: "Total pengguna", value: "1.248", sub: "+32 minggu ini" },
  {
    label: "Total saldo beredar",
    value: "Rp 1,09 M",
    sub: "Seluruh dompet aktif",
  },
  { label: "Transaksi hari ini", value: "58", sub: "3 menunggu" },
];
const recent = [
  { t: "Isi saldo", u: "Rani Putri", amt: 200000, st: ["Berhasil", "success"] },
  {
    t: "Transfer",
    u: "Budi Santoso",
    amt: 150000,
    st: ["Berhasil", "success"],
  },
  { t: "Isi saldo", u: "Sinta Dewi", amt: 500000, st: ["Menunggu", "warning"] },
  { t: "Transfer", u: "Andi Wijaya", amt: 1000000, st: ["Ditolak", "danger"] },
];

export default function Ringkasan() {
  return (
    <AppLayout
      role="admin"
      crumb="Admin"
      title="Ringkasan platform"
      subtitle="Pantau aktivitas seluruh pengguna."
    >
      <div className="mb-5 grid gap-4 sm:grid-cols-3">
        {stats.map((s) => (
          <Card key={s.label} className="p-5!">
            <p className="text-xs text-slate-500">{s.label}</p>
            <p className="mt-1 text-2xl font-semibold text-slate-900">
              {s.value}
            </p>
            <p className="mt-1 text-xs text-teal-700">{s.sub}</p>
          </Card>
        ))}
      </div>

      <Card>
        <CardTitle
          title="Transaksi terbaru"
          right={
            <Link
              to="/admin/pengguna"
              className="text-xs font-medium text-teal-700 hover:underline"
            >
              Lihat semua →
            </Link>
          }
        />
        <Table head={["Jenis", "Pengguna", "Nominal", "Status"]}>
          {recent.map((r, i) => (
            <tr key={i}>
              <Td className="font-medium text-slate-900">{r.t}</Td>
              <Td className="text-slate-500">{r.u}</Td>
              <Td>{rupiah(r.amt)}</Td>
              <Td>
                <Badge tone={r.st[1]}>{r.st[0]}</Badge>
              </Td>
            </tr>
          ))}
        </Table>
      </Card>
    </AppLayout>
  );
}
