import { useEffect, useState } from "react";
import { Link } from "react-router";

import { AppLayout } from "../../components/AppLayout";
import { Card, CardTitle, Badge, Table, Td } from "../../components/ui";

import { useAuth } from "../../context/useAuth";
import { getAdminDashboard } from "../../services/adminService";

const formatDate = (date) => {
  if (!date) return "-";

  return new Intl.DateTimeFormat("id-ID", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    timeZone: "Asia/Jakarta",
  }).format(new Date(date));
};

const formatNumber = (number) =>
  new Intl.NumberFormat("id-ID").format(number ?? 0);

const getStatusInfo = (status) => {
  switch (status?.toUpperCase()) {
    case "ACTIVE":
      return { label: "Aktif", tone: "success" };

    case "SUSPENDED":
      return { label: "Ditangguhkan", tone: "danger" };

    default:
      return { label: status ?? "-", tone: "neutral" };
  }
};

export default function Ringkasan() {
  const { accessToken } = useAuth();

  const [dashboard, setDashboard] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!accessToken) return;

    let isMounted = true;

    const fetchDashboard = async () => {
      try {
        setIsLoading(true);
        setError("");

        const response = await getAdminDashboard(accessToken);

        if (isMounted) {
          setDashboard(response);
        }
      } catch (err) {
        if (isMounted) {
          setError(err.message || "Gagal memuat data dashboard.");
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    fetchDashboard();

    return () => {
      isMounted = false;
    };
  }, [accessToken]);

  const stats = [
    {
      label: "Total pengguna",
      value: dashboard?.totalUsers,
      sub: "Seluruh akun terdaftar",
    },
    {
      label: "Pengguna aktif",
      value: dashboard?.activeUsers,
      sub: "Akun dengan status aktif",
    },
    {
      label: "Ditangguhkan",
      value: dashboard?.suspendedUsers,
      sub: "Akun yang sedang ditangguhkan",
    },
  ];

  const recentUsers = dashboard?.recentUsers ?? [];

  return (
    <AppLayout
      crumb="Admin"
      title="Ringkasan platform"
      subtitle="Pantau kondisi dan aktivitas pengguna platform."
    >
      {/* Statistics */}
      <div className="mb-5 grid gap-4 sm:grid-cols-3">
        {stats.map((stat) => (
          <Card key={stat.label} className="p-5!">
            <p className="text-xs text-slate-500">{stat.label}</p>

            <p className="mt-2 text-2xl font-semibold text-slate-900">
              {isLoading ? "..." : formatNumber(stat.value)}
            </p>

            <p className="mt-1 text-xs text-slate-500">{stat.sub}</p>
          </Card>
        ))}
      </div>

      {/* Recent Users */}
      <Card>
        <CardTitle
          title="Pengguna terbaru"
          right={
            <Link
              to="/pengguna"
              className="text-xs font-medium text-teal-700 hover:underline"
            >
              Lihat semua →
            </Link>
          }
        />

        {error ? (
          <div className="py-8 text-center">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        ) : isLoading ? (
          <div className="py-8 text-center text-sm text-slate-500">
            Memuat data pengguna...
          </div>
        ) : (
          <Table head={["Pengguna", "Tanggal bergabung", "Status"]}>
            {recentUsers.length > 0 ? (
              recentUsers.map((user) => {
                const status = getStatusInfo(user.status);

                return (
                  <tr key={user.id}>
                    <Td>
                      <div>
                        <p className="font-medium text-slate-900">
                          {user.fullName || user.userName}
                        </p>

                        <p className="mt-0.5 text-xs text-slate-500">
                          {user.email}
                        </p>
                      </div>
                    </Td>

                    <Td className="text-slate-500">
                      {formatDate(user.createdAtUtc)}
                    </Td>

                    <Td>
                      <Badge tone={status.tone}>{status.label}</Badge>
                    </Td>
                  </tr>
                );
              })
            ) : (
              <tr>
                <Td colSpan={3} className="py-8 text-center text-slate-500">
                  Belum ada pengguna.
                </Td>
              </tr>
            )}
          </Table>
        )}
      </Card>
    </AppLayout>
  );
}
