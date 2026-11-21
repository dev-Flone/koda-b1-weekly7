
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<Users> Users => Set<Users>();
    public DbSet<Wallets> Wallets => Set<Wallets>();
    public DbSet<Transactions> Transactions => Set<Transactions>();
    public DbSet<WalletMutations> WalletMutations => Set<WalletMutations>();
    public DbSet<PaymentMethods> PaymentMethods => Set<PaymentMethods>();
    public DbSet<TopUpRequests> TopUpRequests => Set<TopUpRequests>();
    public DbSet<AuditLogs> AuditLogs => Set<AuditLogs>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
