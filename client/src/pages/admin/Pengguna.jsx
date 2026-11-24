import { Link } from "react-router";
import { AppLayout } from "../../components/AppLayout";
import {
  Card,
  CardTitle,
  Badge,
  Table,
  Td,
  inputCls,
  rupiah,
} from "../../components/ui";

const users = [
  { id: 1, n: "Rani Putri", e: "rani@mail.com", bal: 1250000, st: "Aktif" },
  { id: 2, n: "Budi Santoso", e: "budi@mail.com", bal: 340000, st: "Aktif" },
  { id: 3, n: "Sinta Dewi", e: "sinta@mail.com", bal: 0, st: "Diblokir" },
  { id: 4, n: "Andi Wijaya", e: "andi@mail.com", bal: 5200000, st: "Aktif" },
  { id: 5, n: "Dewi Lestari", e: "dewi@mail.com", bal: 87000, st: "Aktif" },
];

export default function Pengguna() {
  return (
    <AppLayout
      role="admin"
      crumb="Admin"
      title="Pengguna"
      subtitle="Kelola seluruh akun yang terdaftar."
    >
      <Card>
        <CardTitle
          title="Daftar pengguna"
          desc="Cari, filter, dan buka detail akun."
        />
        <div className="mb-4 grid gap-3 sm:grid-cols-[1fr_150px_150px]">
          <input className={inputCls} placeholder="Cari nama atau email…" />
          <select className={inputCls}>
            <option>Semua status</option>
            <option>Aktif</option>
            <option>Diblokir</option>
          </select>
          <select className={inputCls}>
            <option>Terbaru</option>
            <option>Saldo terbesar</option>
          </select>
        </div>

        <Table head={["Nama", "Email", "Saldo", "Status", ""]}>
          {users.map((u) => (
            <tr key={u.id}>
              <Td className="font-medium text-slate-900">{u.n}</Td>
              <Td className="text-slate-500">{u.e}</Td>
              <Td>{rupiah(u.bal)}</Td>
              <Td>
                <Badge tone={u.st === "Aktif" ? "success" : "danger"}>
                  {u.st}
                </Badge>
              </Td>
              <Td className="text-right">
                <Link
                  to={`/admin/pengguna/${u.id}`}
                  className="text-xs font-medium text-teal-700 hover:underline"
                >
                  Detail
                </Link>
              </Td>
            </tr>
          ))}
        </Table>
        <p className="mt-4 text-xs text-slate-500">
          Menampilkan 5 dari 1.248 pengguna
        </p>
      </Card>
    </AppLayout>
  );
}
