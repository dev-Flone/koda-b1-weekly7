
using EWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Seeding;

public static class UserSeedData
{
    private static readonly (string UserName, string FullName, string Email, string Phone)[] SeedUsers =
    [
        ("seed.andi", "Andi Pratama", "andi.seed@example.com", "081200000001"),
        ("seed.budi", "Budi Santoso", "budi.seed@example.com", "081200000002"),
        ("seed.citra", "Citra Lestari", "citra.seed@example.com", "081200000003"),
        ("seed.dimas", "Dimas Saputra", "dimas.seed@example.com", "081200000004"),
        ("seed.eka", "Eka Putri", "eka.seed@example.com", "081200000005"),
        ("seed.fajar", "Fajar Ramadhan", "fajar.seed@example.com", "081200000006"),
        ("seed.gita", "Gita Maharani", "gita.seed@example.com", "081200000007"),
        ("seed.hendra", "Hendra Wijaya", "hendra.seed@example.com", "081200000008"),
        ("seed.indah", "Indah Permata", "indah.seed@example.com", "081200000009"),
        ("seed.joko", "Joko Susanto", "joko.seed@example.com", "081200000010")
    ];

    public static void Seed(DbContext context)
    {
        var existingUsers = context.Set<Users>().ToList();

        var existingUserNames = existingUsers
            .Select(u => u.UserName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingEmails = existingUsers
            .Select(u => u.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingPhones = existingUsers
            .Select(u => u.Phone)
            .ToHashSet();

        var newUsers = new List<Users>();

        foreach (var seed in SeedUsers)
        {
            if (existingUserNames.Contains(seed.UserName))
                continue;

            if (existingEmails.Contains(seed.Email) ||
                existingPhones.Contains(seed.Phone))
            {
                throw new InvalidOperationException(
                    $"Seed user '{seed.UserName}' conflicts with existing user data.");
            }

            newUsers.Add(new Users(
                seed.UserName,
                seed.FullName,
                seed.Email,
                seed.Phone
            ));

            existingUserNames.Add(seed.UserName);
            existingEmails.Add(seed.Email);
            existingPhones.Add(seed.Phone);
        }

        if (newUsers.Count == 0)
            return;

        context.Set<Users>().AddRange(newUsers);
        context.SaveChanges();
    }

    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingUsers = await context.Set<Users>()
            .ToListAsync(cancellationToken);

        var existingUserNames = existingUsers
            .Select(u => u.UserName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingEmails = existingUsers
            .Select(u => u.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingPhones = existingUsers
            .Select(u => u.Phone)
            .ToHashSet();

        var newUsers = new List<Users>();

        foreach (var seed in SeedUsers)
        {
            if (existingUserNames.Contains(seed.UserName))
                continue;

            if (existingEmails.Contains(seed.Email) ||
                existingPhones.Contains(seed.Phone))
            {
                throw new InvalidOperationException(
                    $"Seed user '{seed.UserName}' conflicts with existing user data.");
            }

            newUsers.Add(new Users(
                seed.UserName,
                seed.FullName,
                seed.Email,
                seed.Phone
            ));

            existingUserNames.Add(seed.UserName);
            existingEmails.Add(seed.Email);
            existingPhones.Add(seed.Phone);
        }

        if (newUsers.Count == 0)
            return;

        await context.Set<Users>()
            .AddRangeAsync(newUsers, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
