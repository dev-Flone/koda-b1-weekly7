import Sidebar from "./layout/Sidebar";
import Header from "./layout/Header";
import { Logo } from "./ui";
import { Link } from "react-router";
import { useAuth } from "../context/useAuth";

export function AppLayout({
  user,
  activeItem,
  crumb,
  eyebrow,
  title,
  subtitle,
  description,
  children,
}) {
  const { user: authUser } = useAuth();

  const currentUser = user || authUser;

  return (
    <div className="app-layout">
      <Sidebar activeItem={activeItem || title || crumb} />

      <main className="main-content">
        <Header
          user={currentUser}
          eyebrow={eyebrow || crumb || "Personal Wallet"}
          title={title || "Beranda"}
          description={description || subtitle}
        />

        {children}
      </main>
    </div>
  );
}

export function FocusLayout({ back, children }) {
  return (
    <div className="min-h-screen bg-white">
      <div className="px-8 py-5">
        <Logo />
      </div>

      <main className="mx-auto w-full max-w-sm px-4 pb-16 pt-6">
        {back && (
          <Link
            to={back.to}
            className="mb-3 inline-block text-xs font-medium text-teal-700 hover:underline"
          >
            {back.label}
          </Link>
        )}
        {children}
      </main>
    </div>
  );
}
