
using Bogus;
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Seeding;

public static class WalletSeedData
{
    public static void Seed(DbContext context)
    {
        var users = context.Set<Users>().ToList();
        var existingWallets = context.Set<Wallets>().ToList();

        var existingUserIds = existingWallets
            .Select(w => w.UserId)
            .ToHashSet();

        var existingWalletNumbers = existingWallets
            .Select(w => w.WalletNumber)
            .ToHashSet();

        var faker = new Faker();
        var newWallets = new List<Wallets>();

        foreach (var user in users)
        {
            if (existingUserIds.Contains(user.Id))
                continue;

            var walletNumber = GenerateWalletNumber(
                faker,
                existingWalletNumbers
            );

            newWallets.Add(new Wallets(
                user.Id,
                walletNumber,
                0m
            ));

            existingUserIds.Add(user.Id);
        }

        if (newWallets.Count == 0)
            return;

        context.Set<Wallets>().AddRange(newWallets);
        context.SaveChanges();
    }

    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var users = await context.Set<Users>()
            .ToListAsync(cancellationToken);

        var existingWallets = await context.Set<Wallets>()
            .ToListAsync(cancellationToken);

        var existingUserIds = existingWallets
            .Select(w => w.UserId)
            .ToHashSet();

        var existingWalletNumbers = existingWallets
            .Select(w => w.WalletNumber)
            .ToHashSet();

        var faker = new Faker();
        var newWallets = new List<Wallets>();

        foreach (var user in users)
        {
            if (existingUserIds.Contains(user.Id))
                continue;

            var walletNumber = GenerateWalletNumber(
                faker,
                existingWalletNumbers
            );

            newWallets.Add(new Wallets(
                user.Id,
                walletNumber,
                0m
            ));

            existingUserIds.Add(user.Id);
        }

        if (newWallets.Count == 0)
            return;

        await context.Set<Wallets>()
            .AddRangeAsync(newWallets, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateWalletNumber(
        Faker faker,
        HashSet<string> existingWalletNumbers)
    {
        string walletNumber;

        do
        {
            walletNumber = $"WLT-{faker.Random.ReplaceNumbers("#######")}";
        }
        while (!existingWalletNumbers.Add(walletNumber));

        return walletNumber;
    }
}
