using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    InTransit,
    Delivered,
    Completed,
    Cancelled
}

public class Order
{
    public int Id { get; set; }

    public required string UserId { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Precision(18, 4)]
    public decimal TotalPrice { get; set; }
    [Precision(18, 9)]
    public decimal TotalEthAmount { get; set; }

    public string? TransactionHash { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? DeliveryAddress { get; set; }
    public string? PhoneNumber { get; set; }
}
