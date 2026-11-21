using EWallet.Domain.Entities;

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

}


