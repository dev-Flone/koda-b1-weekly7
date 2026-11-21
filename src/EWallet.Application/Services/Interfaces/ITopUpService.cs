using EWallet.Application.Dtos.TopUps;

namespace EWallet.Application.Services.Interfaces;

public interface ITopUpService
{
    Task<List<TopUpResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TopUpResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<List<TopUpResponseDto>> GetByWalletIdAsync(long walletId, CancellationToken cancellationToken);
    Task<TopUpResponseDto> CreateAsync(CreateTopUpRequest request, CancellationToken cancellationToken);
    Task<TopUpResponseDto> ConfirmAsync(long id, CancellationToken cancellationToken);
}


