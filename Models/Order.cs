using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled
}

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }
    public decimal TotalEthAmount { get; set; }

    public string? TransactionHash { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
