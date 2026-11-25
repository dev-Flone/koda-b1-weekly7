using EWallet.Application.DTOs.Admin;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;

namespace EWallet.Application.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<Users>> GetAllAsync(CancellationToken cancellationToken);
    Task<Users?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Users?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<Users?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task AddAsync(Users user, CancellationToken cancellationToken);
    void Update(Users user);
    void Delete(Users user);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken, long? excludedUserId = null);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken, long? excludedUserId = null);
    Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken, long? excludedUserId = null);

    // ADMIN DASHBOARD
    Task<int> CountCustomerAsync(CancellationToken cancellationToken);
    Task<int> CountCustomerByStatuAsync(UserStatus status, CancellationToken cancellationToken);
    Task<List<Users>> GetRecentCustomerAsync(int count, CancellationToken cancellationToken);

    // ADMIN USER
    Task<List<AdminUserResponse>> GetAllCustomerAsync(CancellationToken cancellationToken);
}


