const AuthLayout = ({ title, description, children }) => {
  return (
    <main className="min-h-screen w-full">
      <div
        className="
        grid
        grid-cols-1
        md:grid-cols-[42%_58%]
        min-h-screen
        w-full
      "
      >
        {/* Left Panel */}
        <section
          className="
          bg-primary
          text-white
          px-8
          py-12
          md:px-12
          lg:px-16
          flex
          flex-col
          justify-center"
        >
          <div className="w-full max-w-md mx-auto">
            <h1 className="text-3xl lg:text-4xl font-bold leading-tight">
              {title}
            </h1>

            <p className="mt-4 text-sm lg:text-base text-white/70 leading-relaxed">
              {description}
            </p>
          </div>
        </section>

        {/* Right Panel */}
        <section
          className="
          bg-white
          flex
          items-center
          justify-center
          px-8
          py-12
          md:px-12
          lg:px-20
        "
        >
          <div className="w-full">{children}</div>
        </section>
      </div>
    </main>
  );
};

export default AuthLayout;
