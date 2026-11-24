import { NavLink, Link } from "react-router-dom";
import { Logo, Avatar } from "./ui";

const NAV = {
  customer: [
    { to: "/dashboard", label: "Beranda" },
    { to: "/topup", label: "Isi saldo" },
    { to: "/transfer", label: "Transfer" },
    { to: "/aktivitas", label: "Aktivitas" },
    { to: "/pengaturan", label: "Pengaturan" },
  ],
  admin: [
    { to: "/admin", label: "Ringkasan", end: true },
    { to: "/admin/pengguna", label: "Pengguna" },
  ],
};

// Sidebar + header. Pakai untuk halaman customer & admin (role="admin").
export function AppLayout({ role = "customer", crumb, title, subtitle, children }) {
  const user = role === "admin" ? { name: "Admin Utama", sub: "admin@dompetku.id" } : { name: "Rani Putri", sub: "rani@mail.com" };
  return (
    <div className="flex min-h-screen bg-slate-50">
      <aside className="sticky top-0 hidden h-screen w-56 shrink-0 flex-col border-r border-slate-100 bg-white p-4 md:flex">
        <Logo className="px-2 pb-6 pt-1" />
        <p className="px-2 pb-2 text-[10px] font-semibold uppercase tracking-wider text-slate-400">
          {role === "admin" ? "Admin" : "Menu"}
        </p>
        <nav className="flex flex-1 flex-col gap-1">
          {NAV[role].map((n) => (
            <NavLink
              key={n.to}
              to={n.to}
              end={n.end}
              className={({ isActive }) =>
                `rounded-lg px-3 py-2 text-sm transition ${
                  isActive ? "bg-teal-50 font-medium text-teal-800" : "text-slate-500 hover:bg-slate-50"
                }`
              }
            >
              {n.label}
            </NavLink>
          ))}
        </nav>
        <div className="rounded-xl bg-teal-50 p-3">
          <p className="text-[10px] uppercase tracking-wider text-slate-400">Masuk sebagai</p>
          <p className="mt-1 text-sm font-medium text-slate-900">{user.name}</p>
          <p className="text-xs text-slate-500">{user.sub}</p>
        </div>
      </aside>

      <div className="min-w-0 flex-1 px-5 py-6 md:px-8">
        <header className="mb-6 flex items-start justify-between gap-4">
          <div>
            {crumb && <p className="text-xs font-medium text-teal-700">{crumb}</p>}
            <h1 className="text-xl font-semibold text-slate-900">{title}</h1>
            {subtitle && <p className="mt-0.5 text-sm text-slate-500">{subtitle}</p>}
          </div>
          <Link to="/login" className="flex items-center gap-2 rounded-full bg-white py-1 pl-1 pr-3 text-xs text-slate-600 shadow-sm ring-1 ring-slate-100">
            <Avatar name={user.name} size="h-7 w-7" />
            Keluar
          </Link>
        </header>
        {children}
      </div>
    </div>
  );
}

// Layout tanpa sidebar untuk halaman konfirmasi / sukses / QRIS.
export function FocusLayout({ back, children }) {
  return (
    <div className="min-h-screen bg-white">
      <div className="px-8 py-5"><Logo /></div>
      <main className="mx-auto w-full max-w-sm px-4 pb-16 pt-6">
        {back && (
          <Link to={back.to} className="mb-3 inline-block text-xs font-medium text-teal-700 hover:underline">
            ← {back.label}
          </Link>
        )}
        {children}
      </main>
    </div>
  );
}
