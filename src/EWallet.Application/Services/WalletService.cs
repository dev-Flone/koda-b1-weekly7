
using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.Dtos.Wallets;
using EWallet.Application.DTOs.Wallets;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services;

public sealed class WalletService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IWalletRepository repository,
    IAuditLogService auditLogService
) : IWalletService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<List<WalletResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var wallets = await repository.GetAllAsync(cancellationToken);

        return [.. wallets.Select(MapToResponse)];
    }

    public async Task<WalletResponseDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByIdAsync(id, cancellationToken);

        return wallet is null ? null : MapToResponse(wallet);
    }

    public async Task<List<WalletResponseDto>> GetByStatus(
        WalletStatus status,
        CancellationToken cancellationToken)
    {
        var wallets = await repository.GetByStatus(status, cancellationToken);

        return [.. wallets.Select(MapToResponse)];
    }

    public async Task<WalletResponseDto?> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByUserId(userId, cancellationToken);

        return wallet is null ? null : MapToResponse(wallet);
    }

    public async Task<WalletResponseDto?> GetByWalletNumberAsync(
        string walletNumber,
        CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByWalletNumberAsync(
            walletNumber, cancellationToken);

        return wallet is null ? null : MapToResponse(wallet);
    }

    public async Task<WalletResponseDto?> UpdateStatusAsync(
        long id,
        WalletStatusUpdateDto walletStatusUpdate,
        CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByIdAsync(id, cancellationToken);

        if (wallet is null) return null;

        if (wallet.Status == walletStatusUpdate.Status) return MapToResponse(wallet);

        var prevStatus = wallet.Status;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            wallet.UpdateStatus(walletStatusUpdate.Status);

            repository.Update(wallet);

            var auditLog = new AuditLogs(
                actorUserId: null,
                transactionId: null,
                action: AuditLogAction.WALLET_STATUS_CHANGED,
                type: AuditLogEntityType.WALLET,
                entityId: wallet.Id.ToString(),
                outcome: AuditLogOutcome.SUCCESS,
                description: $"Wallet {wallet.Id} status changed from {prevStatus} to {wallet.Status}.",
                correlationId: null
            );

            await auditLogService.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return MapToResponse(wallet);
        }
        catch (System.Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static WalletResponseDto MapToResponse(Wallets wallet)
    {
        return new WalletResponseDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance,
            Status = wallet.Status.ToString(),
            CreatedAtUtc = wallet.CreatedAtUtc,
            UpdatedAtUtc = wallet.UpdatedAtUtc
        };
    }

    public async Task<RecipientWalletResponse?> GetRecipientByEmailAsync(
    string email,
    CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken
        );

        if (user is null) return null;

        var wallet = await repository.GetByUserId(
            user.Id,
            cancellationToken
        );

        if (wallet is null) return null;

        return new RecipientWalletResponse
        {
            WalletId = wallet.Id,
            WalletNumber = wallet.WalletNumber,
            FullName = user.FullName,
            Email = user.Email,
            Status = wallet.Status.ToString()
        };
    }

    public async Task<RecipientWalletResponse?> GetRecipientByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByWalletNumberAsync(walletNumber, cancellationToken);

        if (wallet is null) return null;

        var user = await _userRepository.GetByIdAsync(wallet.UserId, cancellationToken);

        if (user is null) return null;

        return new RecipientWalletResponse
        {
            WalletId = wallet.Id,
            WalletNumber = wallet.WalletNumber,
            FullName = user.FullName,
            Email = user.Email,
            Status = wallet.Status.ToString()
        };
    }
}
