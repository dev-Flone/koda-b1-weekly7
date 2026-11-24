import { Link } from "react-router";

const QuickActions = ({ onTopUp, onTransfer }) => {
  return (
    <section className="quick-actions card">
      <div>
        <h2>Aksi cepat</h2>
        <p>Transaksi yang sering digunakan</p>
      </div>

      <div className="quick-action-buttons">
        <Link
          to="/topup"
          className="action-button primary capitalize"
          type="button"
          onClick={onTopUp}
        >
          + Isi Saldo
        </Link>

        <Link
          to="/transfer"
          className="action-button secondary capitalize"
          type="button"
          onClick={onTransfer}
        >
          ↗ Transfer
        </Link>
      </div>
    </section>
  );
};

export default QuickActions;
