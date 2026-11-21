const formatCurrency = (amount) => {
  return new Intl.NumberFormat("id-ID", {
    style: "currency",
    currency: "IDR",
    maximumFractionDigits: 0,
  }).format(amount);
};

const formatDate = (dateValue) => {
  if (!dateValue) return "-";

  // Backend mengirim DateTime UTC tanpa penanda Z.
  // Jika belum ada timezone, perlakukan nilainya sebagai UTC.
  const hasTimezone = /Z$|[+-]\d{2}:\d{2}$/.test(dateValue);
  const date = new Date(hasTimezone ? dateValue : `${dateValue}Z`);

  if (Number.isNaN(date.getTime())) return "-";

  return new Intl.DateTimeFormat("id-ID", {
    timeZone: "Asia/Jakarta",
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
};

const getTransactionType = (type) => {
  switch (type?.toUpperCase()) {
    case "TOPUP":
      return "Top Up";
    case "TRANSFER":
      return "Transfer";
    default:
      return type ?? "Transaksi";
  }
};

const getTransactionStatus = (status) => {
  switch (status?.toUpperCase()) {
    case "SUCCESS":
      return "Berhasil";
    case "PENDING":
      return "Menunggu";
    case "FAILED":
      return "Gagal";
    default:
      return status ?? "-";
  }
};

const RecentTransactions = ({ transactions = [] }) => {
  const safeTransactions = Array.isArray(transactions) ? transactions : [];

  return (
    <section className="transactions-card card">
      <div className="section-heading">
        <div>
          <h2>Aktivitas terakhir</h2>
          <p>Pantau pergerakan uang di dompetmu</p>
        </div>

        <a href="#" onClick={(event) => event.preventDefault()}>
          Lihat semua <span>→</span>
        </a>
      </div>

      <div className="transaction-table-wrapper">
        <table className="transaction-table">
          <thead>
            <tr>
              <th>Aktivitas</th>
              <th>Jenis</th>
              <th>Nominal</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {safeTransactions.length === 0 ? (
              <tr>
                <td colSpan="4">Belum ada transaksi.</td>
              </tr>
            ) : (
              safeTransactions.slice(0, 5).map((transaction) => {
                const isIncome =
                  transaction.type?.toUpperCase() === "TOPUP" ||
                  transaction.receiverWalletId === 21;

                const direction = isIncome ? "income" : "expense";

                return (
                  <tr key={transaction.id}>
                    <td>
                      <span className="text-[14px] text-primary-dark font-semibold">
                        {transaction.description ||
                          getTransactionType(transaction.type)}
                      </span>

                      <span className="transaction-date">
                        {formatDate(transaction.createdAtUtc)}
                      </span>
                    </td>

                    <td className="transaction-type">
                      {getTransactionType(transaction.type)}
                    </td>

                    <td className={`transaction-amount ${direction}`}>
                      {isIncome ? "+ " : "- "}
                      {formatCurrency(transaction.amount)}
                    </td>

                    <td className="transaction-status">
                      {getTransactionStatus(transaction.status)}
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
};

export default RecentTransactions;
