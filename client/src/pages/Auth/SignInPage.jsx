import { useState } from "react";
import { Link, useNavigate } from "react-router";

import AuthLayout from "../../components/auth/AuthLayout";
import AuthFormField from "../../components/auth/AuthFormField";
import { useAuth } from "../../context/useAuth";

const SignInPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    setIsLoading(true);
    setError("");

    try {
      await login(formData);

      navigate("/");
    } catch (error) {
      setError(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AuthLayout
      title="Uangmu, lebih terarah."
      description="Kelola keuanganmu dengan lebih mudah, aman, dan praktis dalam satu aplikasi."
    >
      <div className="mb-7">
        <h2 className="text-xl md:text-2xl font-semibold text-primary-dark">
          Selamat datang kembali
        </h2>

        <p className="text-muted mt-2 text-sm">
          Masuk untuk melanjutkan ke akunmu.
        </p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-5">
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
          label="Password"
          name="password"
          type="password"
          placeholder="Masukkan password"
          value={formData.password}
          onChange={handleChange}
          autoComplete="current-password"
        />

        <div className="flex justify-end">
          <Link
            to="/forgot-password"
            className="text-xs font-medium text-primary hover:underline"
          >
            Lupa password?
          </Link>
        </div>

        <button
          type="submit"
          disabled={isLoading}
          className="w-full bg-primary text-white py-3 rounded-sm text-sm font-medium hover:opacity-90 transition-opacity disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? "Sedang masuk..." : "Masuk"}
        </button>
      </form>

      {error && <p className="text-sm text-red-500">{error}</p>}

      <div className="mt-6 text-center text-sm text-muted">
        Belum memiliki akun?{" "}
        <Link to="/signup" className="font-medium text-primary hover:underline">
          Daftar sekarang
        </Link>
      </div>
    </AuthLayout>
  );
};

export default SignInPage;
