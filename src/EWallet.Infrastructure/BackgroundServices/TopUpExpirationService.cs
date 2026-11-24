using EWallet.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EWallet.Infrastructure.BackgroundServices;

public sealed class TopUpExpirationService(IServiceScopeFactory scopeFactory, ILogger<TopUpExpirationService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<TopUpExpirationService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var topUpService = scope.ServiceProvider.GetRequiredService<ITopUpService>();

                _logger.LogInformation("topup expiration check started ad {Time}", DateTime.UtcNow);

                var expiredCount = await topUpService.ExpirePendingAsync(cancellationToken);

                _logger.LogInformation("Expired pending topup requests: {Count}", expiredCount);

                if (expiredCount > 0)
                {
                    _logger.LogInformation("Expired {Count} pending topup request", expiredCount);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured while expiring topup requests.");
            }

            try
            {
                await timer.WaitForNextTickAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

    }

}
