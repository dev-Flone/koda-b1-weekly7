using EWallet.Application.Dtos.PaymentMethods;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services.Interfaces;

public interface IPaymentMethodService
{
    Task<List<PaymentMethodResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<PaymentMethodResponseDto>> GetActiveAsync(CancellationToken cancellationToken);
    Task<PaymentMethodResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PaymentMethodResponseDto?> GetByCodeAsync(PaymentMethodCode code, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);
}


