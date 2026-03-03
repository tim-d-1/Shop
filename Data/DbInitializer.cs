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

        if (!context.Products.Any())
        {
            var products = new List<Product>
                {
                    new() { Name = "Lenovo LOQ 15ARP9", Description = "High-performance gaming laptop with optimized cooling.", Price = 950.00m, StockQuantity = 10, Category = "Hardware", ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=Lenovo+LOQ" },
                    new() { Name = "Mercusys MR80X Wi-Fi 6 Router", Description = "Gigabit router for stable, low-latency connections.", Price = 60.00m, StockQuantity = 25, Category = "Hardware", ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=Mercusys+Router" },
                    new() { Name = "1TB NVMe M.2 Gen4 SSD", Description = "Ultra-fast storage for operating systems and large files.", Price = 85.00m, StockQuantity = 50, Category = "Hardware", ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=1TB+NVMe+SSD" },
                    new() { Name = "Image-Line Harmless Plugin", Description = "Additive synthesizer plugin for digital audio workstations.", Price = 100.00m, StockQuantity = 999, Category = "Software", ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=Harmless+Plugin" },
                    new() { Name = "Mechanical Keypad", Description = "2-key rapid trigger mechanical keypad for rhythm games.", Price = 35.00m, StockQuantity = 15, Category = "Gaming", ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=Keypad" },
                    new() { Name = "Coffee Mug", Description = "250ml plain mug", Price = 2.49m, StockQuantity = 50, ImageUrl = "https://placehold.co/600x400/212529/FFFFFF?text=Coffee Mug"}
                };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

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