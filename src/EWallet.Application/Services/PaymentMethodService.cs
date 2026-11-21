using EWallet.Application.Dtos.PaymentMethods;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;

namespace EWallet.Application.Services;

public sealed class PaymentMethodService(
    IPaymentMethodRepository paymentMethodRepository
) : IPaymentMethodService
{
    public async Task<List<PaymentMethodResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var paymentMethods = await paymentMethodRepository.GetAllAsync(
            cancellationToken);

        return paymentMethods
            .Select(MapToResponseDto)
            .ToList();
    }

    public async Task<List<PaymentMethodResponseDto>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var paymentMethods = await paymentMethodRepository.GetActiveAsync(
            cancellationToken);

        return paymentMethods
            .Select(MapToResponseDto)
            .ToList();
    }


    public async Task<PaymentMethodResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var paymentMethod = await paymentMethodRepository.GetByIdAsync(
            id,
            cancellationToken);

        return paymentMethod is null
            ? null
            : MapToResponseDto(paymentMethod);
    }

    public async Task<PaymentMethodResponseDto?> GetByCodeAsync(
        PaymentMethodCode code,
        CancellationToken cancellationToken = default)
    {
        var paymentMethod = await paymentMethodRepository.GetByCodeAsync(
            code,
            cancellationToken);

        return paymentMethod is null
            ? null
            : MapToResponseDto(paymentMethod);
    }

    public Task<bool> ExistsByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return paymentMethodRepository.ExistsByIdAsync(
            id,
            cancellationToken);
    }

    private static PaymentMethodResponseDto MapToResponseDto(
    PaymentMethods paymentMethod)
    {
        return new PaymentMethodResponseDto(
            paymentMethod.Id,
            paymentMethod.Code.ToString(),
            paymentMethod.Name,
            paymentMethod.ProviderName,
            paymentMethod.IsActive,
            paymentMethod.CreatedAtUtc
        );
    }

}


