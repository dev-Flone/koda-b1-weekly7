const AuthFormField = ({
  label,
  name,
  type = "text",
  placeholder,
  value,
  onChange,
  error,
  autoComplete,
}) => {
  return (
    <div>
      {/* label */}
      <label
        htmlFor={name}
        className="block text-sm font-medium text-muted mb-2"
      >
        {label}
      </label>

      {/* input */}
      <input
        id={name}
        name={name}
        type={type}
        placeholder={placeholder}
        value={value}
        onChange={onChange}
        autoComplete={autoComplete}
        className={`w-full px-4 py-3 text-sm text-primary-dark placeholder:text-[#1D2928] bg-white border rounded-md outline-none transition-colors focus:border-[#126B63] focus:ring-2 focus:ring-[#126B63]/10 ${error ? "border-red-500" : "border-[#E3E8E6]"}`}
      />

      {error && <p className="mt-1 text-xs text-red-500">{error}</p>}
    </div>
  );
};

export default AuthFormField;
