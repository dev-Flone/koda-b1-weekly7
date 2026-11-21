const Header = () => {
  return (
    <header className="dashboard-header">
      <div className="page-heading">
        <span className="eyebrow uppercase">personal wallet</span>
        <h1 className="capitalize">beranda</h1>
        <p>
          <span className="capitalize">
            Selasa, 17 November 2026 • Selamat datang kembali.
          </span>
        </p>
      </div>

      <button className="profile-button" type="button">
        <span className="profile-avatar">NP</span>
        <span className="profile-name">Nadia</span>
      </button>
    </header>
  );
};

export default Header;
