using EWallet.Application.Dtos.Transactions;
using EWallet.Application.Dtos.Transfer;

namespace EWallet.Application.Services.Interfaces;

public interface ITransferService
{
    Task<List<TransferResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TransferResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<List<TransferResponseDto>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken);
    Task<TransferResponseDto> CreateAsync(CreateTransferRequestDto request, CancellationToken cancellationToken);
}


