using System.Globalization;
using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.Dtos.Users;
using EWallet.Application.DTOs.Admin;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;

namespace EWallet.Application.Services;

public sealed class UserService(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IWalletRepository walletRepository,
    IWalletNumberGenerator walletNumberGenerator,
    IAuditLogService auditLogService
) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UserResponseDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByUsernameAsync(
            request.UserName, cancellationToken))
        {
            throw new InvalidOperationException(
                "Username is already registered.");
        }

        if (await userRepository.ExistsByEmailAsync(
            request.Email, cancellationToken))
        {
            throw new InvalidOperationException(
                "Email is already registered.");
        }

        if (await userRepository.ExistsByPhoneAsync(
            request.Phone, cancellationToken))
        {
            throw new InvalidOperationException(
                "Phone number is already registered.");
        }

        string walletNumber;

        do
        {
            walletNumber = walletNumberGenerator.Generate();
        }
        while (await walletRepository.ExistsByWalletNumber(
            walletNumber, cancellationToken));

        await using var transaction =
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new Users(
                request.UserName,
                request.FullName,
                request.Email,
                request.Phone
            );

            await userRepository.AddAsync(user, cancellationToken);

            // Save User to obtain its generated ID.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Automatically create Wallet for the new User.
            var wallet = new Wallets(
                user.Id,
                walletNumber,
                0m
            );

            await walletRepository.AddAsync(wallet, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return MapToResponse(user);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            id, cancellationToken);

        if (user is null)
            return false;

        if (user.Status == UserStatus.SUSPENDED)
            return true;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user.Suspend();

            userRepository.Update(user);

            var auditLog = new AuditLogs(
                actorUserId: null,
                transactionId: null,
                action: AuditLogAction.USER_SUSPENDED,
                type: AuditLogEntityType.USER,
                entityId: user.Id.ToString(),
                outcome: AuditLogOutcome.SUCCESS,
                description: $"User {user.Id} was suspended.",
                correlationId: null
            );

            await auditLogService.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<UserResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);

        return [.. users.Select(MapToResponse)];
    }

    public async Task<UserResponseDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            id, cancellationToken);

        return user is null ? null : MapToResponse(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(
        long id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            id, cancellationToken);

        if (user is null)
            return null;

        if (await userRepository.ExistsByUsernameAsync(
            request.UserName, cancellationToken, id))
        {
            throw new InvalidOperationException(
                "Username is already registered.");
        }

        if (await userRepository.ExistsByEmailAsync(
            request.Email, cancellationToken, id))
        {
            throw new InvalidOperationException(
                "Email is already registered.");
        }

        if (await userRepository.ExistsByPhoneAsync(
            request.Phone, cancellationToken, id))
        {
            throw new InvalidOperationException(
                "Phone number is already registered.");
        }

        user.UpdateProfile(
            request.UserName,
            request.FullName,
            request.Email,
            request.Phone
        );

        userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    private static UserResponseDto MapToResponse(Users user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.Phone,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var totalUsers = await userRepository.CountCustomerAsync(cancellationToken);

        var activeUsers = await userRepository.CountCustomerByStatuAsync(UserStatus.ACTIVE, cancellationToken);

        var suspendedUsers = await userRepository.CountCustomerByStatuAsync(UserStatus.SUSPENDED, cancellationToken);

        var recentUsers = await userRepository.GetRecentCustomerAsync(5, cancellationToken);

        var recentUserResponse = recentUsers.Select(user => new RecentUser(
            user.Id,
            user.UserName ?? string.Empty,
            user.FullName,
            user.Email ?? string.Empty,
            user.Status.ToString(),
            user.CreatedAtUtc
        )).ToList();

        return new AdminDashboardResponse(
            totalUsers,
            activeUsers,
            suspendedUsers,
            recentUserResponse
        );
    }

    public async Task<List<AdminUserResponse>> GetCustomerAsync(CancellationToken cancellationToken)
    {
        return await userRepository.GetAllCustomerAsync(cancellationToken);
    }
}
