using EWallet.Application.Abstractions.Persistence;
using EWallet.Application.DTOs.Auth;
using EWallet.Application.Repositories.Interfaces;
using EWallet.Application.Services.Interfaces;
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace EWallet.Application.Services;

public class AuthService(
    UserManager<Users> userManager,
    SignInManager<Users> signInManager,
    IUnitOfWork unitOfWork,
    IWalletRepository walletRepository,
    IUserRepository userRepository,
    IWalletNumberGenerator walletNumberGenerator,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenService refreshTokenService
) : IAuthService
{
    private const int TokenLifetimeMinutes = 15;
    public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || user.Status != UserStatus.ACTIVE) throw new InvalidOperationException("Invalid email or password");

        var result = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true
        );

        if (!result.Succeeded) throw new InvalidOperationException("Invalid email or password");

        var rawToken = refreshTokenService.GenerateToken();
        var tokenExpiry = refreshTokenService.GetExpiryDate();

        var refreshToken = new RefreshTokens(
            user.Id,
            Guid.NewGuid(),
            refreshTokenService.HashToken(rawToken),
            tokenExpiry
        );

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreateAuthResponse(user, rawToken, tokenExpiry);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenService.HashToken(refreshToken);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var storedToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is not null && storedToken.IsActive)
        {
            var activeTokens = await refreshTokenRepository.GetActiveByFamilyIdAsync(storedToken.FamilyId, cancellationToken);

            foreach (var activeToken in activeTokens)
            {
                activeToken.Revoke();
                refreshTokenRepository.Update(activeToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenService.HashToken(refreshToken);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var storedToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken.IsRevoked)
        {
            var activeTokens = await refreshTokenRepository.GetActiveByFamilyIdAsync(storedToken.FamilyId, cancellationToken);

            foreach (var activeToken in activeTokens)
            {
                activeToken.Revoke();
                refreshTokenRepository.Update(activeToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            throw new UnauthorizedAccessException("Refresh Token reuse detected. Session revoked");
        }

        if (storedToken.IsExpired) throw new UnauthorizedAccessException("Refresh token has expiree.");

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());

        if (user is null || user.Status != UserStatus.ACTIVE) throw new UnauthorizedAccessException("User is not authorized");

        var newRawToken = refreshTokenService.GenerateToken();
        var newTokenHash = refreshTokenService.HashToken(newRawToken);
        var newTokenExpiry = refreshTokenService.GetExpiryDate();

        storedToken.Revoke(newTokenHash);
        refreshTokenRepository.Update(storedToken);

        var newRefreshToken = new RefreshTokens(
            user.Id,
            storedToken.FamilyId,
            newTokenHash,
            newTokenExpiry
        );

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreateAuthResponse(user, newRawToken, newTokenExpiry);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByNameAsync(request.UserName) is not null) throw new InvalidOperationException("Username is already registered.");

        if (await userManager.FindByEmailAsync(request.Email) is not null) throw new InvalidOperationException("Email is already registered.");

        if (await userRepository.ExistsByPhoneAsync(request.Phone, cancellationToken)) throw new InvalidOperationException("Phone Number is already registered.");

        string walletNumber;

        do
        {
            walletNumber = walletNumberGenerator.Generate();
        } while (await walletRepository.ExistsByWalletNumber(walletNumber, cancellationToken));

        var rawToken = refreshTokenService.GenerateToken();
        var tokenExpiry = refreshTokenService.GetExpiryDate();
        var familyId = Guid.NewGuid();

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new Users(
                request.UserName,
                request.FullName,
                request.Email,
                request.Phone
            );

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(errors => errors.Description));

                throw new InvalidOperationException(errors);
            }

            var wallet = new Wallets(
                user.Id,
                walletNumber,
                0m
            );

            await walletRepository.AddAsync(wallet, cancellationToken);

            var refreshToken = new RefreshTokens(
                user.Id,
                familyId,
                refreshTokenService.HashToken(rawToken),
                tokenExpiry
            );

            await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CreateAuthResponse(user, rawToken, tokenExpiry);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // Methods
    private AuthResponseDto CreateAuthResponse(Users user, string refreshToken, DateTime refreshTokenExpiry)
    {
        return new AuthResponseDto(
            AccessToken: jwtTokenService.GenerateToken(user),
            ExpiresAtUtc: DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes),
            RefreshToken: refreshToken,
            RefreshTokenExpiresAtUtc: refreshTokenExpiry,
            UserId: user.Id,
            UserName: user.UserName ?? string.Empty,
            FullName: user.FullName,
            Email: user.Email ?? string.Empty
        );
    }
}
