using EWallet.Application.Dtos.Wallets;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services.Interfaces;

public interface IWalletService
{
    Task<List<WalletResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<WalletResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<WalletResponseDto?> GetByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task<WalletResponseDto?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken);
    Task<List<WalletResponseDto>> GetByStatus(WalletStatus status, CancellationToken cancellationToken);
    Task<WalletResponseDto?> UpdateStatusAsync(long id, WalletStatusUpdateDto walletStatusUpdate, CancellationToken cancellationToken);
}



