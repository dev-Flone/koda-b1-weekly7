using EWallet.Application.Dtos.Users;

namespace EWallet.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponseDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<UserResponseDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserResponseDto?> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}



