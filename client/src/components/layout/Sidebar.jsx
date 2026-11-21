const menuItems = [
  {
    label: "Beranda",
    icon: "⌂",
    active: true,
  },
  {
    label: "Isi Saldo",
    icon: "+",
  },
  {
    label: "Transfer",
    icon: "↗",
  },
  {
    label: "Aktivitas",
    icon: "◷",
  },
  {
    label: "Pengaturan",
    icon: "⚙",
  },
];

const Sidebar = () => {
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
        <p className="sidebar-label uppercase">menu utama</p>

        <nav className="sidebar-nav">
          {menuItems.map((item) => (
            <a
              href="#"
              key={item.label}
              className={`nav-item ${item.active ? "active" : ""}`}
              aria-current={item.active ? "page" : undefined}
              onClick={(event) => event.preventDefault()}
            >
              <span className="nav-label">{item.label}</span>
            </a>
          ))}

          <a
            href="#"
            className="nav-item"
            onClick={(event) => event.preventDefault()}
          >
            <span className="font-semibold text-[14px]">Keluar</span>
          </a>
        </nav>
      </div>

      <div className="sidebar-help">
        <span className="help-label uppercase">butuh bantuan?</span>
        <strong>Pusat Bantuan</strong>
        <p>Panduan dan informasi akun</p>
      </div>
    </aside>
  );
};

export default Sidebar;
