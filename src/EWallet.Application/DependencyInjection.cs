
using EWallet.Application.Services;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace EWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITopUpService, TopUpService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IWalletMutationService, WalletMutationService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        services.AddSingleton<IWalletNumberGenerator, WalletNumberGenerator>();

        return services;
    }
}
