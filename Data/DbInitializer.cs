namespace InternetShop.Data;

using InternetShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();
        string[] roleNames = { "SuperAdmin", "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        string superAdminEmail = "admin@example.com";
        string superAdminPassword = "password1234!";

        var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdminUser == null)
        {
            var newSuperAdmin = new IdentityUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true
            };

            var createPowerUser = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);

            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
            }
            else
            {
                var errorMessages = string.Join(", ", createPowerUser.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create SuperAdmin. Errors: {errorMessages}");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(superAdminUser, "SuperAdmin"))
            {
                await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
            }
        }
    }
}