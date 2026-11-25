import { NavLink, useNavigate } from "react-router";
import { useAuth } from "../../context/useAuth";

const menuItems = [
  { label: "Ringkasan", path: "/ringkasan" },
  { label: "Pengguna", path: "/pengguna" },
];

const AdminSidebar = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/signin", { replace: true });
  };

  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-logo">k</div>

        <span className="font-bold text-primary-dark text-[22px]">
          koda{" "}
          <span className="text-primary font-semibold text-[22px]">wallet</span>
        </span>
      </div>

      <div className="sidebar-content">
        <p className="sidebar-label uppercase">administrasi</p>

        <nav className="sidebar-nav">
          {menuItems.map((item) => (
            <NavLink
              to={item.path}
              key={item.label}
              className={({ isActive }) =>
                `nav-item ${isActive ? "active" : ""}`
              }
            >
              <span className="nav-icon">{item.icon}</span>
              <span className="nav-label">{item.label}</span>
            </NavLink>
          ))}

          <button type="button" className="logout" onClick={handleLogout}>
            <span className="font-semibold text-[14px]">Keluar</span>
          </button>
        </nav>
      </div>

      <div className="sidebar-help">
        <span className="help-label uppercase">administrator</span>
        <strong>Panel Admin</strong>
        <p>Kelola pengguna dan aktivitas platform</p>
      </div>
    </aside>
  );
};

export default AdminSidebar;
