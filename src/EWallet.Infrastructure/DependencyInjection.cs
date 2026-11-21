using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Infrastructure.Authentication;
using EWallet.Infrastructure.Persistence;
using EWallet.Infrastructure.Repositories;
using EWallet.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EWalletDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'EWalletDb' was not found.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString)
                .UseSeeding((context, _) => Seed(context))
                .UseAsyncSeeding((context, _, cancellationToken) =>
                    SeedAsync(context, cancellationToken));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITopUpRequestRepository, TopUpRequestRepository>();
        services.AddScoped<IWalletMutationRepository, WalletMutationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddIdentityCore<Users>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<IdentityRole<long>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return services;
    }

    public static async Task MigrateDbAsync(
        this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
    }

    private static void Seed(DbContext context)
    {
        UserSeedData.Seed(context);
        WalletSeedData.Seed(context);
        TransactionSeedData.Seed(context);
    }

    private static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        await UserSeedData.SeedAsync(context, cancellationToken);
        await WalletSeedData.SeedAsync(context, cancellationToken);
        await TransactionSeedData.SeedAsync(context, cancellationToken);
    }
}
