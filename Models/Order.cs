using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    [Precision(18, 4)]
    public decimal TotalPrice { get; set; }
    [Precision(18, 9)]
    public decimal TotalEthAmount { get; set; }

    public string? TransactionHash { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
