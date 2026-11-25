import ProtectedRoute from "./components/ProtectedRoute";
import AuthProvider from "./context/AuthProvider";
import SignInPage from "./pages/Auth/SignInPage";
import SignUpPage from "./pages/Auth/SignUpPage";
import DashboardPage from "./pages/DashboardPage";
import { BrowserRouter, Routes, Route } from "react-router";
import TopUpPage from "./pages/TopUpPage";
import Aktivitas from "./pages/customer/Aktivitas";
import Pengaturan from "./pages/customer/Pengaturan";
import TransferKonfirmasi from "./pages/customer/TransferKonfirmasi";
import TransferBerhasil from "./pages/customer/TransferBerhasil";
import TopUpKonfirmasi from "./pages/customer/TopupKonfirmasi";
import TopupBerhasil from "./pages/customer/TopupBerhasil";
import TopupQris from "./pages/customer/TopupQris";
import Ringkasan from "./pages/admin/Ringkasan";
import Pengguna from "./pages/admin/Pengguna";
import DetailPengguna from "./pages/admin/DetailPengguna";
import TopUpTransfer from "./pages/customer/TopUpTransfer";
import TransferPage from "./pages/TransferPage";
import RoleProtectedRoute from "./components/RoleProtectedRoute";

const App = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Publid Routes */}
          <Route path="/signin" element={<SignInPage />} />
          <Route path="/signup" element={<SignUpPage />} />

          {/* Protected Routes */}
          <Route element={<ProtectedRoute />}>
            {/* Customer Routes */}
            <Route element={<RoleProtectedRoute allowedRole="CUSTOMER" />}>
              <Route path="/" element={<DashboardPage />} />
              <Route path="/topup" element={<TopUpPage />} />
              <Route path="/topup/konfirmasi" element={<TopUpKonfirmasi />} />
              <Route path="/topup/berhasil" element={<TopupBerhasil />} />
              <Route path="/topup/qris" element={<TopupQris />} />
              <Route path="/topup/transfer" element={<TopUpTransfer />} />
              <Route path="/transfer" element={<TransferPage />} />
              <Route
                path="/transfer/konfirmasi"
                element={<TransferKonfirmasi />}
              />
              <Route path="/transfer/berhasil" element={<TransferBerhasil />} />
              <Route path="/aktivitas" element={<Aktivitas />} />
              <Route path="/pengaturan" element={<Pengaturan />} />
            </Route>

            <Route element={<RoleProtectedRoute allowedRole="ADMIN" />}>
              <Route path="/ringkasan" element={<Ringkasan />} />
              <Route path="/pengguna" element={<Pengguna />} />
              <Route path="/pengguna/:id" element={<DetailPengguna />} />
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
};

export default App;
