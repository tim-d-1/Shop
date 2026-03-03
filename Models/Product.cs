using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Precision(18, 9)]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public string Category { get; set; } = "General";
}
