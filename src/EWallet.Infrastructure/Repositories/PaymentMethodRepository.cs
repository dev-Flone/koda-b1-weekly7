using EWallet.Infrastructure.Persistence;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public sealed class PaymentMethodRepository(AppDbContext context) : IPaymentMethodRepository
{
    public async Task<List<PaymentMethods>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await context.PaymentMethods.AsNoTracking().Where(pm => pm.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentMethods>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.PaymentMethods.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<PaymentMethods?> GetByCodeAsync(PaymentMethodCode code, CancellationToken cancellationToken)
    {
        return await context.PaymentMethods.FirstOrDefaultAsync(pm => pm.Code == code, cancellationToken);
    }

    public async Task<PaymentMethods?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.PaymentMethods.FirstOrDefaultAsync(pm => pm.Id == id, cancellationToken);
    }


    // EXISTS
    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.PaymentMethods.AnyAsync(pm => pm.Id == id, cancellationToken);
    }
}


