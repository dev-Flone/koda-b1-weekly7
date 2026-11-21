const LoginPage = () => {
  return (
    <div className="bg-slate-300 min-h-screen flex items-center">
      <div className="flex flex-col justify-center gap-4 w-lg bg-white mx-auto py-12 px-6 rounded-2xl">
        <div>
          <h1 className="text-3xl font-semibold">Login</h1>
          <p className="mt-3">Login page EWallet</p>
        </div>

        <form action="">
          <div className="flex flex-col">
            <label htmlFor="">Email</label>
            <input type="text" />
          </div>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
