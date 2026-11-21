
using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.Dtos.TopUps;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services;

public sealed class TopUpService(
    IUnitOfWork unitOfWork,
    ITopUpRequestRepository topUpRequestRepository,
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository,
    IPaymentMethodRepository paymentMethodRepository,
    IWalletMutationRepository walletMutationRepository,
    IAuditLogService auditLogService
) : ITopUpService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TopUpResponseDto> CreateAsync(CreateTopUpRequest request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new ArgumentException(
                "Top up amount must be greater than zero.");

        var wallet = await walletRepository.GetByIdAsync(
            request.WalletId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Wallet was not found.");

        if (wallet.Status != WalletStatus.ACTIVE)
            throw new InvalidOperationException("Wallet is not active.");

        var paymentMethod = await paymentMethodRepository.GetByIdAsync(
            request.PaymentMethodId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Payment method was not found.");

        if (!paymentMethod.IsActive)
            throw new InvalidOperationException("Payment method is not active.");

        await using var dbTransaction =
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var transaction = new Transactions(
                referenceNo: GenerateReferenceNo(),
                receiverWalletId: wallet.Id,
                amount: request.Amount,
                type: TransactionType.TOPUP,
                status: TransactionStatus.PENDING,
                description: $"Top Up via {paymentMethod.Name}"
            );

            await transactionRepository.AddAsync(
                transaction,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var topUpRequest = new TopUpRequests(
                transactionId: transaction.Id,
                walletId: wallet.Id,
                paymentMethodId: paymentMethod.Id,
                amount: request.Amount,
                status: TopUpRequestStatus.PENDING
            );

            await topUpRequestRepository.AddAsync(
                topUpRequest,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await dbTransaction.CommitAsync(cancellationToken);

            return MapToResponse(topUpRequest);
        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<TopUpResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var requests = await topUpRequestRepository.GetAllAsync(
            cancellationToken);

        return [.. requests.Select(MapToResponse)];
    }

    public async Task<List<TopUpResponseDto>> GetByWalletIdAsync(long id, CancellationToken cancellationToken)
    {
        var requests = await topUpRequestRepository.GetByWalletIdAsync(
            id,
            cancellationToken);

        return [.. requests.Select(MapToResponse)];
    }

    public async Task<TopUpResponseDto> ConfirmAsync(long id, CancellationToken cancellationToken)
    {
        await using var dbTransaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var topUpRequest = await topUpRequestRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Top-up request was not found.");

            if (topUpRequest.Status != TopUpRequestStatus.PENDING) throw new InvalidOperationException("Only pending top-up requests can be confirmed.");

            var transaction = await transactionRepository.GetByIdAsync(topUpRequest.TransactionId, cancellationToken) ?? throw new KeyNotFoundException("Transaction was not found.");

            var wallet = await walletRepository.GetByIdAsync(topUpRequest.WalletId, cancellationToken) ?? throw new KeyNotFoundException("Wallet was not found.");

            if (wallet.Status != WalletStatus.ACTIVE) throw new InvalidOperationException("Wallet is not active.");

            topUpRequest.Confirmation();
            transaction.Confirmation();

            var balanceBefore = wallet.Balance;
            wallet.Credit(topUpRequest.Amount);

            var balanceAfer = wallet.Balance;

            var mutation = new WalletMutations(
                transactionId: transaction.Id,
                walletId: wallet.Id,
                amount: topUpRequest.Amount,
                balanceBefore: balanceBefore,
                balanceAfter: balanceAfer,
                direction: WalletMutationDirection.IN
            );

            await walletMutationRepository.AddAsync(mutation, cancellationToken);

            var auditLog = new AuditLogs(
                actorUserId: null,
                transactionId: transaction.Id,
                action: AuditLogAction.TOPUP_PAID,
                type: AuditLogEntityType.TOPUPREQUEST,
                entityId: topUpRequest.Id.ToString(),
                outcome: AuditLogOutcome.SUCCESS,
                description: $"Top-up request {topUpRequest.Id} confirmed successfully.",
                correlationId: null
            );

            await auditLogService.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await dbTransaction.CommitAsync(cancellationToken);

            return MapToResponse(topUpRequest);

        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<TopUpResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var request = await topUpRequestRepository.GetByIdAsync(
            id,
            cancellationToken);

        return request is null ? null : MapToResponse(request);
    }

    private static string GenerateReferenceNo()
    {
        return $"TOP{DateTime.UtcNow:yyyyMMddHHmmssfff}" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }

    private static TopUpResponseDto MapToResponse(TopUpRequests request)
    {
        return new TopUpResponseDto
        {
            Id = request.Id,
            TransactionId = request.TransactionId,
            WalletId = request.WalletId,
            PaymentMethodId = request.PaymentMethodId,
            Amount = request.Amount,
            Status = request.Status.ToString(),
            ProviderReference = request.ProviderReference,
            CreatedAtUtc = request.CreatedAtUtc,
            ExpiresAtUtc = request.ExpiresAtUtc,
            PaidAtUtc = request.PaidAtUtc
        };
    }
}
