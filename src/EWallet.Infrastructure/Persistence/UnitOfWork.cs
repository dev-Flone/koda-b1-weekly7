
using EWallet.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace EWallet.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        return new EfUnitOfWorkTransaction(transaction);
    }

    private sealed class EfUnitOfWorkTransaction(
        IDbContextTransaction transaction
    ) : IUnitOfWorkTransaction
    {
        public Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(
            CancellationToken cancellationToken = default)
        {
            return transaction.RollbackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return transaction.DisposeAsync();
        }
    }
}
