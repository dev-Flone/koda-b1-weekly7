import { useState } from "react";
import AuthFormField from "../../components/auth/AuthFormField";
import AuthLayout from "../../components/auth/AuthLayout";
import { Link, useNavigate } from "react-router";
import { register } from "../../services/authService";

const SignUpPage = () => {
  const [formData, setFormData] = useState({
    userName: "",
    fullName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });

  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");

    if (formData.password !== formData.confirmPassword) {
      setError("Password dan confirm password tidak sama");
      return;
    }

    const request = {
      userName: formData.userName,
      fullName: formData.fullName,
      email: formData.email,
      phone: formData.phone,
      password: formData.password,
    };

    try {
      setIsLoading(true);

      await register(request);

      alert("Registrasi berhasil! Silakan masuk ke akun anda");

      navigate("/signin");
    } catch (err) {
      setError(err.message || "Registrasi gagal. Silakan coba lagi nanti");
    } finally {
      setIsLoading(false);
    }
  };
  return (
    <AuthLayout
      title="Satu akun, satu dompet."
      description="Mulai perjalanan finansialmu dengan pengelolaan uang yang lebih sederhana."
    >
      <div className="w-full">
        <div className="mb-6">
          <h2 className="text-[30px] font-semibold text-[#202625]">
            Buat akun KodaWallet
          </h2>

          <p className="mt-2 text-sm text-[#858D8B]">
            Isi informasi berikut untuk memulai.
          </p>
        </div>

        <form
          onSubmit={handleSubmit}
          className="space-y-4 grid grid-cols-2 gap-x-5 gap-y-3"
        >
          <AuthFormField
            label="Nama lengkap"
            name="fullName"
            placeholder="Masukkan nama lengkap"
            value={formData.fullName}
            onChange={handleChange}
            autoComplete="name"
          />

          <AuthFormField
            label="Username"
            name="userName"
            placeholder="Masukkan username"
            value={formData.userName}
            onChange={handleChange}
            autoComplete="username"
          />

          <AuthFormField
            label="Email"
            name="email"
            type="email"
            placeholder="Masukkan email"
            value={formData.email}
            onChange={handleChange}
            autoComplete="email"
          />

          <AuthFormField
            label="Nomor telepon"
            name="phone"
            type="tel"
            placeholder="Masukkan nomor telepon"
            value={formData.phone}
            onChange={handleChange}
            autoComplete="tel"
          />

          <AuthFormField
            label="Password"
            name="password"
            type="password"
            placeholder="Buat password"
            value={formData.password}
            onChange={handleChange}
            autoComplete="new-password"
          />

          <AuthFormField
            label="Konfirmasi password"
            name="confirmPassword"
            type="password"
            placeholder="Ulangi password"
            value={formData.confirmPassword}
            onChange={handleChange}
            autoComplete="new-password"
          />

          <p className="text-muted">
            Akun baru akan mendapatkan dompet aktif dengan saldo Rp0.
          </p>

          {error && <p className="col-span-2 text-sm text-red-600">{error}</p>}

          <button
            type="submit"
            disabled={isLoading}
            className="w-full text-left px-6 bg-primary text-white py-4 rounded-[10px] text-sm font-medium  col-span-2 hover:bg-[#0E5952] transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
          >
            {isLoading ? "Mendaftarkan akun..." : "Buat akun"}
          </button>
        </form>

        <div className="mt-5 text-center text-sm text-primary font-semibold">
          Sudah memiliki akun?{" "}
          <Link
            to="/signin"
            className="font-medium text-[#126B63] hover:underline"
          >
            Masuk
          </Link>
        </div>
      </div>
    </AuthLayout>
  );
};

export default SignUpPage;
