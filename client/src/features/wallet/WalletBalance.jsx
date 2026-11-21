const WalletBalance = ({ wallet }) => {
  const balance = Number(wallet?.balance ?? 0);

  return (
    <section className="balance-card">
      <span className="card-eyebrow uppercase">saldo tersedia</span>

      <h2>
        {new Intl.NumberFormat("id-ID", {
          style: "currency",
          currency: "IDR",
          maximumFractionDigits: 0,
        }).format(Number.isNaN(balance) ? 0 : balance)}
      </h2>

      <div className="wallet-number">
        <span>Nomor dompet · {wallet?.walletNumber ?? "-"}</span>
      </div>

      <button className="account-status" type="button">
        <span className="status-dot" />
        {wallet?.status === "ACTIVE"
          ? "●  Aktif"
          : wallet?.status === "FROZEN"
            ? "●  Dibekukan"
            : wallet?.status === "CLOSED"
              ? "●  Ditutup"
              : (wallet?.status ?? "-")}
      </button>
    </section>
  );
};

export default WalletBalance;
