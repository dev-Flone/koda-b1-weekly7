const Header = ({
  user,
  eyebrow = "personal wallet",
  title = "beranda",
  description,
}) => {
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
        <span className="eyebrow uppercase">{eyebrow}</span>

        <h1 className="capitalize">{title}</h1>

        <p>{description || `Selamat datang kembali, ${fullName}`}</p>
      </div>

      <button className="profile-button" type="button">
        <span className="profile-avatar">{initials}</span>
        <span className="profile-name">{fullName}</span>
      </button>
    </header>
  );
};

export default Header;
