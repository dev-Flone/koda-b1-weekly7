const Header = ({ user }) => {
  const fullName = user?.fullName || "Pengguna";

  const initials = fullName
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((word) => word.charAt(0).toUpperCase())
    .join("");

  return (
    <header className="dashboard-header">
      <div className="page-heading">
        <span className="eyebrow uppercase">personal wallet</span>
        <h1 className="capitalize">beranda</h1>
        <p>
          <span className="capitalize">
            Selasa, 17 November 2026 • Selamat datang kembali, {fullName}
          </span>
        </p>
      </div>

      <button className="profile-button" type="button">
        <span className="profile-avatar">{initials}</span>
        <span className="profile-name">{fullName}</span>
      </button>
    </header>
  );
};

export default Header;
