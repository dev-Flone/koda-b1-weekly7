import { useEffect, useMemo, useState } from "react";
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
import { useAuth } from "../../context/useAuth";
import { getAdminUsers } from "../../services/adminService";

export default function Pengguna() {
  const { accessToken } = useAuth();

  const [users, setUsers] = useState([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("ALL");
  const [sortBy, setSortBy] = useState("NEWEST");

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!accessToken) return;

    const fetchUsers = async () => {
      try {
        setIsLoading(true);
        setError("");

        const response = await getAdminUsers(accessToken);
        setUsers(response);
      } catch (err) {
        setError(err.message || "Gagal mengambil data pengguna.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchUsers();
  }, [accessToken]);

  const filteredUsers = useMemo(() => {
    let result = [...users];

    // Search by name or email
    if (search.trim()) {
      const keyword = search.toLowerCase().trim();

      result = result.filter(
        (user) =>
          user.fullName.toLowerCase().includes(keyword) ||
          user.email.toLowerCase().includes(keyword),
      );
    }

    // Filter by status
    if (statusFilter !== "ALL") {
      result = result.filter((user) => user.status === statusFilter);
    }

    // Sorting
    if (sortBy === "NEWEST") {
      result.sort(
        (a, b) => new Date(b.createdAtUtc) - new Date(a.createdAtUtc),
      );
    } else if (sortBy === "BALANCE") {
      result.sort((a, b) => b.walletBalance - a.walletBalance);
    }

    return result;
  }, [users, search, statusFilter, sortBy]);

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
          <input
            className={inputCls}
            placeholder="Cari nama atau email…"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />

          <select
            className={inputCls}
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
          >
            <option value="ALL">Semua status</option>
            <option value="ACTIVE">Aktif</option>
            <option value="SUSPENDED">Ditangguhkan</option>
          </select>

          <select
            className={inputCls}
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
          >
            <option value="NEWEST">Terbaru</option>
            <option value="BALANCE">Saldo terbesar</option>
          </select>
        </div>

        {isLoading ? (
          <p className="py-8 text-center text-sm text-slate-500">
            Memuat data pengguna...
          </p>
        ) : error ? (
          <p className="py-8 text-center text-sm text-red-600">{error}</p>
        ) : filteredUsers.length === 0 ? (
          <p className="py-8 text-center text-sm text-slate-500">
            Tidak ada pengguna yang ditemukan.
          </p>
        ) : (
          <Table head={["Nama", "Email", "Saldo", "Status", ""]}>
            {filteredUsers.map((user) => (
              <tr key={user.id}>
                <Td className="font-medium text-slate-900">{user.fullName}</Td>

                <Td className="text-slate-500">{user.email}</Td>

                <Td>{rupiah(user.walletBalance)}</Td>

                <Td>
                  <Badge tone={user.status === "ACTIVE" ? "success" : "danger"}>
                    {user.status === "ACTIVE" ? "Aktif" : "Ditangguhkan"}
                  </Badge>
                </Td>

                <Td className="text-right">
                  <Link
                    to={`/pengguna/${user.id}`}
                    className="text-xs font-medium text-teal-700 hover:underline"
                  >
                    Detail
                  </Link>
                </Td>
              </tr>
            ))}
          </Table>
        )}

        {!isLoading && !error && (
          <p className="mt-4 text-xs text-slate-500">
            Menampilkan {filteredUsers.length} dari {users.length} pengguna
          </p>
        )}
      </Card>
    </AppLayout>
  );
}
