import { AppLayout } from "../../components/AppLayout";
import { Card, CardTitle, Field, Input, Button } from "../../components/ui";

export default function Pengaturan() {
  return (
    <AppLayout
      crumb="Akun"
      activeItem="Pengaturan"
      title="Pengaturan"
      subtitle="Kelola profil dan keamanan akun kamu."
    >
      <div className="grid gap-5 lg:grid-cols-[1fr_300px]">
        <Card>
          <CardTitle
            title="Informasi akun"
            desc="Perbarui data diri yang tampil di profil."
          />
          <form className="space-y-4">
            <Field label="Nama lengkap">
              <Input defaultValue="Rani Putri" />
            </Field>
            <Field label="Email">
              <Input type="email" defaultValue="rani@mail.com" />
            </Field>
            <Field label="Nomor telepon">
              <Input defaultValue="0812 3456 7890" />
            </Field>
            <Field label="Kata sandi">
              <Input type="password" defaultValue="••••••••" />
            </Field>
            <Button type="button">Simpan perubahan</Button>
          </form>
        </Card>

        <Card className="h-fit">
          <CardTitle title="Keamanan" />
          <p className="text-xs text-slate-500">Kata sandi terakhir diubah</p>
          <p className="mb-1 text-sm font-medium text-slate-900">
            2 bulan lalu
          </p>
          <p className="mb-4 text-xs text-slate-500">
            Ganti kata sandi secara berkala agar akun tetap aman.
          </p>
          <Button variant="soft">Ganti kata sandi</Button>
        </Card>
      </div>
    </AppLayout>
  );
}
