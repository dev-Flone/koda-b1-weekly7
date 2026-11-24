import { Link } from "react-router-dom";

export const rupiah = (n) => "Rp " + Number(n).toLocaleString("id-ID");

export function Logo({ className = "" }) {
  return (
    <Link to="/" className={`flex items-center gap-2 ${className}`}>
      <span className="grid h-7 w-7 place-items-center rounded-full bg-teal-700 text-xs font-bold text-white">D</span>
      <span className="font-semibold text-slate-900">
        Dompet<span className="text-teal-700">ku</span>
      </span>
    </Link>
  );
}

export function Card({ className = "", children }) {
  return <div className={`rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-100 ${className}`}>{children}</div>;
}

export function CardTitle({ title, desc, right }) {
  return (
    <div className="mb-5 flex items-start justify-between gap-4">
      <div>
        <h2 className="text-sm font-semibold text-slate-900">{title}</h2>
        {desc && <p className="mt-0.5 text-xs text-slate-500">{desc}</p>}
      </div>
      {right}
    </div>
  );
}

const btn = {
  primary: "bg-teal-700 text-white hover:bg-teal-800",
  soft: "bg-teal-50 text-teal-800 hover:bg-teal-100",
  danger: "bg-red-700 text-white hover:bg-red-800",
  ghost: "text-teal-700 hover:underline",
};
export function Button({ variant = "primary", className = "", as: As = "button", ...props }) {
  return (
    <As
      className={`inline-flex w-full items-center justify-center rounded-lg px-4 py-2.5 text-sm font-medium transition ${btn[variant]} ${className}`}
      {...props}
    />
  );
}

const badge = {
  success: "bg-emerald-50 text-emerald-700",
  warning: "bg-amber-50 text-amber-700",
  danger: "bg-red-50 text-red-600",
  neutral: "bg-slate-100 text-slate-600",
};
export function Badge({ tone = "success", children }) {
  return <span className={`inline-block rounded-md px-2 py-0.5 text-xs font-medium ${badge[tone]}`}>{children}</span>;
}

export function Field({ label, hint, children }) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-xs font-medium text-slate-600">{label}</span>
      {children}
      {hint && <span className="mt-1 block text-xs text-slate-400">{hint}</span>}
    </label>
  );
}

export const inputCls =
  "w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-900 placeholder:text-slate-400 focus:border-teal-600 focus:bg-white focus:outline-none focus:ring-2 focus:ring-teal-600/20";

export function Input(props) {
  return <input className={inputCls} {...props} />;
}

export function Row({ label, value, strong }) {
  return (
    <div className="flex items-center justify-between gap-4 py-2 text-sm">
      <span className="text-slate-500">{label}</span>
      <span className={strong ? "text-base font-semibold text-teal-700" : "font-medium text-slate-900"}>{value}</span>
    </div>
  );
}

export function Avatar({ name, size = "h-10 w-10" }) {
  const ini = name.split(" ").map((s) => s[0]).slice(0, 2).join("").toUpperCase();
  return (
    <span className={`grid ${size} shrink-0 place-items-center rounded-full bg-teal-700 text-sm font-semibold text-white`}>
      {ini}
    </span>
  );
}

export function Table({ head, children }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-slate-100 text-xs text-slate-400">
            {head.map((h, i) => (
              <th key={i} className="px-2 py-2.5 font-medium">{h}</th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">{children}</tbody>
      </table>
    </div>
  );
}
export const Td = ({ className = "", ...p }) => <td className={`px-2 py-3 ${className}`} {...p} />;
