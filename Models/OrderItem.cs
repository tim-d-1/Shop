using Microsoft.EntityFrameworkCore;

namespace InternetShop.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }
    [Precision(18, 9)]
    public decimal PriceAtPurchase { get; set; }
}
