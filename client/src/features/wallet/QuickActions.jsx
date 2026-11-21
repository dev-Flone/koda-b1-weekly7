const QuickActions = ({ onTopUp, onTransfer }) => {
  return (
    <section className="quick-actions card">
      <div>
        <h2>Aksi cepat</h2>
        <p>Transaksi yang sering digunakan</p>
      </div>

      <div className="quick-action-buttons">
        <button
          className="action-button primary capitalize"
          type="button"
          onClick={onTopUp}
        >
          + Isi Saldo
        </button>

        <button
          className="action-button secondary capitalize"
          type="button"
          onClick={onTransfer}
        >
          ↗ Transfer
        </button>
      </div>
    </section>
  );
};

export default QuickActions;
