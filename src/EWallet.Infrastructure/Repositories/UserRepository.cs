using EWallet.Infrastructure.Persistence;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EWallet.Domain.Constants;
using EWallet.Domain.Enums;
using EWallet.Application.DTOs.Admin;

namespace EWallet.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<List<Users>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Users.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Users?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Users?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Users?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }


    // Exists    
    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken, long? excludedUserId = null)
    {
        return await context.Users.AnyAsync(u => u.UserName == username && (!excludedUserId.HasValue || u.Id != excludedUserId.Value), cancellationToken);
    }
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken, long? excludedUserId = null)
    {
        return await context.Users.AnyAsync(u => u.Email == email && (!excludedUserId.HasValue || u.Id != excludedUserId.Value), cancellationToken);
    }
    public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken, long? excludedUserId = null)
    {
        return await context.Users.AnyAsync(u => u.PhoneNumber == phone && (!excludedUserId.HasValue || u.Id != excludedUserId.Value), cancellationToken);
    }



    // METHODS
    public async Task AddAsync(Users user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(Users user)
    {
        context.Users.Update(user);
    }

    public void Delete(Users user)
    {
        context.Users.Remove(user);
    }

    // ADMIN

    private IQueryable<Users> GetCustomersQuery()
    {
        return
            from user in context.Users.AsNoTracking()
            join userRole in context.UserRoles
                on user.Id equals userRole.UserId
            join role in context.Roles
                on userRole.RoleId equals role.Id
            where role.Name == AppRoles.CUSTOMER
            select user;
    }

    public async Task<int> CountCustomerAsync(CancellationToken cancellationToken)
    {
        return await GetCustomersQuery().CountAsync(cancellationToken);
    }

    public async Task<int> CountCustomerByStatuAsync(UserStatus status, CancellationToken cancellationToken)
    {
        return await GetCustomersQuery().CountAsync(user => user.Status == status, cancellationToken);
    }

    public async Task<List<Users>> GetRecentCustomerAsync(int count, CancellationToken cancellationToken)
    {
        return await GetCustomersQuery().OrderByDescending(user => user.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdminUserResponse>> GetAllCustomerAsync(CancellationToken cancellationToken)
    {
        return await GetCustomersQuery()
            .OrderByDescending(user => user.CreatedAtUtc)
            .Select(user => new AdminUserResponse(
                user.Id,
                user.UserName!,
                user.FullName,
                user.Email!,
                user.Status.ToString(),
                user.Wallets == null ? 0m : user.Wallets.Balance,
                user.CreatedAtUtc
            )).ToListAsync();
    }
}


