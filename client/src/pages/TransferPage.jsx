import { AppLayout } from "../components/AppLayout";

const TransferPage = () => {
  return (
    <div className="app-layout">
      <AppLayout
        crumb="Transaksi"
        activeItem="Transfer"
        title="Transfer"
        subtitle="Kirim saldo denganmudah ke pengguna lainnya."
      >
        <div className="grid grid-cols-1 xl:grid-cols-[minmax(0,1fr)_280px] gap-5">
          {/* Form Transfer */}
          <section className="bg-white rounded-xl p-6 md:p-7">
            <div className="mb-7">
              <h2 className="text-base font-semibold text-primary-dark">
                Buat transfer baru
              </h2>

              <p className="text-xs text-muted mt-1">
                Masukkan informasi penerima dan nominal transfer.
              </p>
            </div>

            <form>
              {/* Penerima */}
              <div className="mb-6">
                <label
                  htmlFor="recipient"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Penerima
                </label>

                <input
                  id="recipient"
                  type="text"
                  placeholder="Masukkan nomor wallet atau email penerima"
                  className="w-full border border-gray-200 rounded-lg px-4 py-3 text-sm outline-none focus:border-primary transition"
                />
              </div>

              {/* Nominal */}
              <div className="mb-6">
                <label
                  htmlFor="transfer-amount"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Nominal transfer
                </label>

                <div className="border border-gray-200 rounded-lg px-4 py-3 flex items-center">
                  <span className="text-xs text-muted mr-2">Rp</span>

                  <input
                    id="transfer-amount"
                    type="number"
                    placeholder="0"
                    className="w-full outline-none text-sm font-semibold text-primary-dark"
                  />
                </div>
              </div>

              {/* Catatan */}
              <div className="mb-8">
                <label
                  htmlFor="transfer-note"
                  className="block text-xs font-medium text-muted mb-2"
                >
                  Catatan (opsional)
                </label>

                <textarea
                  id="transfer-note"
                  rows={3}
                  placeholder="Tambahkan catatan untuk penerima"
                  className="w-full resize-none border border-gray-200 rounded-lg px-4 py-3 text-sm outline-none focus:border-primary transition"
                />
              </div>

              <button
                type="button"
                className="w-full rounded-lg bg-primary text-white text-xs font-semibold py-3 hover:opacity-90 transition"
              >
                Lanjutkan transfer
              </button>
            </form>
          </section>

          {/* Ringkasan Akun */}
          <aside className="bg-white rounded-xl p-6 h-fit">
            <h2 className="text-sm font-semibold text-primary-dark">
              Ringkasan akun
            </h2>

            <div className="mt-5">
              <p className="text-xs text-muted mb-2">Saldo tersedia</p>

              <p className="text-xl font-bold text-primary-dark">Rp 0</p>
            </div>

            <div className="border-t border-gray-100 mt-5 pt-4 flex items-center justify-between">
              <span className="text-xs text-muted">Status akun</span>

              <span className="rounded-full bg-emerald-50 text-emerald-700 px-3 py-1 text-[10px] font-semibold uppercase">
                Aktif
              </span>
            </div>
          </aside>
        </div>
      </AppLayout>
    </div>
  );
};

export default TransferPage;
