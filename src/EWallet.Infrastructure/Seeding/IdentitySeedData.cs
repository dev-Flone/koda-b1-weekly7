
using EWallet.Domain.Constants;
using EWallet.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EWallet.Infrastructure.Seeding;

public static class IdentitySeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

        var userManager =
            serviceProvider.GetRequiredService<UserManager<Users>>();

        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        // 1. Create roles if they do not exist.
        foreach (var roleName in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new IdentityRole<long>(roleName);

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role {roleName}: {errors}");
            }
        }

        // 2. Read admin configuration.
        var adminSettings = configuration.GetSection("AdminSeed");

        var userName = adminSettings["UserName"];
        var fullName = adminSettings["FullName"];
        var email = adminSettings["Email"];
        var phone = adminSettings["Phone"];
        var password = adminSettings["Password"];

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(phone) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "AdminSeed configuration is incomplete.");
        }

        // 3. Find or create admin account.
        var admin = await userManager.FindByEmailAsync(email);

        if (admin is null)
        {
            admin = new Users(
                userName,
                fullName,
                email,
                phone
            );

            var createResult = await userManager.CreateAsync(
                admin,
                password
            );

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create admin account: {errors}");
            }
        }

        // 4. Ensure the configured account has ADMIN role.
        if (!await userManager.IsInRoleAsync(admin, AppRoles.ADMIN))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin,
                AppRoles.ADMIN
            );

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to assign ADMIN role: {errors}");
            }
        }

        var users = await userManager.Users.ToListAsync();

        foreach (var existingUser in users)
        {
            if (existingUser.Id == admin.Id) continue;

            var existingRoles = await userManager.GetRolesAsync(existingUser);

            if (existingRoles.Count > 0) continue;

            var customerRoleResult = await userManager.AddToRoleAsync(existingUser, AppRoles.CUSTOMER);

            if (!customerRoleResult.Succeeded)
            {
                var errors = string.Join("; ", customerRoleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException($"Failed to assign CUSTOMER role to user {existingUser.Id}: {errors}");
            }
        }
    }
}
