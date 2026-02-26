namespace InternetShop.Data;

using InternetShop.Models;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Products.Any())
        {
            return;
        }

        var products = new Product[]
        {
                new Product { Name = "Ethereum Graphic Tee", Price = 25.00m, Description = "100% Cotton with ETH Logo", StockQuantity = 50 },
                new Product { Name = "Ledger Nano X", Price = 149.00m, Description = "Hardware wallet for your ETH", StockQuantity = 10 },
                new Product { Name = "Crypto Mug", Price = 15.00m, Description = "Drink coffee, trade ETH", StockQuantity = 100 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}