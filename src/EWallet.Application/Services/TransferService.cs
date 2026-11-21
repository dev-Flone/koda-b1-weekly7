
using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.Dtos.Transfer;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services;

public sealed class TransferService(
    IUnitOfWork unitOfWork,
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository,
    IWalletMutationRepository walletMutationRepository
) : ITransferService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IWalletMutationRepository _walletMutationRepository = walletMutationRepository;

    public async Task<TransferResponseDto> CreateAsync(
        CreateTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.SenderWalletId == request.ReceiverWalletId)
            throw new InvalidOperationException(
                "You cannot send to your own wallet.");

        if (request.Amount <= 0)
            throw new ArgumentException(
                "Transfer amount must be greater than 0.");

        var sender = await _walletRepository.GetByIdAsync(
            request.SenderWalletId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Sender wallet was not found.");

        var receiver = await _walletRepository.GetByIdAsync(
            request.ReceiverWalletId,
            cancellationToken)
            ?? throw new KeyNotFoundException("Receiver wallet was not found.");

        if (sender.Status != WalletStatus.ACTIVE ||
            receiver.Status != WalletStatus.ACTIVE)
        {
            throw new InvalidOperationException(
                "Sender and Receiver wallet should be active.");
        }

        if (sender.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient Balance.");

        await using var dbTransaction =
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var senderBalanceBefore = sender.Balance;
            var receiverBalanceBefore = receiver.Balance;

            var senderBalanceAfter = senderBalanceBefore - request.Amount;
            var receiverBalanceAfter = receiverBalanceBefore + request.Amount;

            var transaction = new Transactions(
                referenceNo: GenerateReferenceNo(),
                receiverWalletId: receiver.Id,
                amount: request.Amount,
                type: TransactionType.TRANSFER,
                senderWalletId: sender.Id,
                status: TransactionStatus.SUCCESS,
                description: request.Description,
                completedAtUtc: DateTime.UtcNow
            );

            await _transactionRepository.AddAsync(
                transaction,
                cancellationToken);

            // Simpan transaksi agar TransactionId tersedia.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Ubah saldo melalui domain methods.
            sender.Debit(request.Amount);
            receiver.Credit(request.Amount);

            _walletRepository.Update(sender);
            _walletRepository.Update(receiver);

            // Catat perubahan saldo.
            var senderMutation = new WalletMutations(
                transaction.Id,
                sender.Id,
                request.Amount,
                senderBalanceBefore,
                senderBalanceAfter,
                WalletMutationDirection.OUT
            );

            var receiverMutation = new WalletMutations(
                transaction.Id,
                receiver.Id,
                request.Amount,
                receiverBalanceBefore,
                receiverBalanceAfter,
                WalletMutationDirection.IN
            );

            await _walletMutationRepository.AddAsync(
                senderMutation,
                cancellationToken);

            await _walletMutationRepository.AddAsync(
                receiverMutation,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await dbTransaction.CommitAsync(cancellationToken);

            return MapToResponse(transaction);
        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<TransferResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(
            cancellationToken);

        return transactions
            .Where(t => t.Type == TransactionType.TRANSFER)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TransferResponseDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (transaction is null ||
            transaction.Type != TransactionType.TRANSFER)
        {
            return null;
        }

        return MapToResponse(transaction);
    }

    public async Task<List<TransferResponseDto>> GetByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByWalletId(
            walletId,
            cancellationToken);

        return transactions
            .Where(t => t.Type == TransactionType.TRANSFER)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(MapToResponse)
            .ToList();
    }

    private static TransferResponseDto MapToResponse(
        Transactions transaction)
    {
        return new TransferResponseDto
        {
            TransactionId = transaction.Id,
            ReferenceNo = transaction.ReferenceNo,
            SenderWalletId = transaction.SenderWalletId!.Value,
            ReceiverWalletId = transaction.ReceiverWalletId,
            Amount = transaction.Amount,
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            CreatedAtUtc = transaction.CreatedAtUtc,
            CompletedAtUtc = transaction.CompletedAtUtc
        };
    }

    private static string GenerateReferenceNo()
    {
        return $"TRF-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..35]
            .ToUpperInvariant();
    }
}
