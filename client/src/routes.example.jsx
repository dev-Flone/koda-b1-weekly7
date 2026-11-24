// Tambahkan ke router yang sudah kamu punya (react-router-dom v6/v7).
import Transfer from "./pages/customer/Transfer";
import TransferKonfirmasi from "./pages/customer/TransferKonfirmasi";
import TransferBerhasil from "./pages/customer/TransferBerhasil";
import Aktivitas from "./pages/customer/Aktivitas";
import Pengaturan from "./pages/customer/Pengaturan";
import TopupMetode from "./pages/customer/TopupMetode";
import TopupKonfirmasi from "./pages/customer/TopupKonfirmasi";
import TopupQris from "./pages/customer/TopupQris";
import TopupBerhasil from "./pages/customer/TopupBerhasil";
import Ringkasan from "./pages/admin/Ringkasan";
import Pengguna from "./pages/admin/Pengguna";
import DetailPengguna from "./pages/admin/DetailPengguna";

export const extraRoutes = [
  { path: "/transfer", element: <Transfer /> },
  { path: "/transfer/konfirmasi", element: <TransferKonfirmasi /> },
  { path: "/transfer/berhasil", element: <TransferBerhasil /> },
  { path: "/aktivitas", element: <Aktivitas /> },
  { path: "/pengaturan", element: <Pengaturan /> },
  { path: "/topup/metode", element: <TopupMetode /> },
  { path: "/topup/konfirmasi", element: <TopupKonfirmasi /> },
  { path: "/topup/qris", element: <TopupQris /> },
  { path: "/topup/berhasil", element: <TopupBerhasil /> },
  { path: "/admin", element: <Ringkasan /> },
  { path: "/admin/pengguna", element: <Pengguna /> },
  { path: "/admin/pengguna/:id", element: <DetailPengguna /> },
];
