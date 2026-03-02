using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Services;
using InternetShop.Models;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace InternetShop.Pages.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string ContractAddress { get; set; } = string.Empty;
        public string ContractABI { get; set; } = string.Empty;
        private readonly CoinGeckoService _coinGeckoService;
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context, CoinGeckoService coinGeckoService, IConfiguration config)
        {
            _context = context;
            _coinGeckoService = coinGeckoService;

            ContractAddress = config["EthereumSettings:ContractAddress"] ?? "";
            ContractABI = config["EthereumSettings:ContractABI"] ?? "[]";
        }

        public IList<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalFiatPrice { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                TotalFiatPrice = CartItems.Sum(item => item.Product!.Price * item.Quantity);
            }
        }
        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEthPriceAsync()
        {
            var rate = await _coinGeckoService.GetEthToUsdRateAsync();
            return new JsonResult(new { usdRate = rate });
        }

        public async Task<IActionResult> OnPostCheckoutAsync(string transactionHash)
        {
            // 1. If the form submitted without a hash, just reload the page
            if (string.IsNullOrEmpty(transactionHash))
            {
                return RedirectToPage("/Cart/Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            // 2. Fetch the cart items
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToPage("/Cart/Index");

            // 3. Calculate final totals for the official receipt
            decimal fiatTotal = cartItems.Sum(item => item.Product!.Price * item.Quantity);
            var currentEthRate = await _coinGeckoService.GetEthToUsdRateAsync();
            decimal ethTotal = fiatTotal / currentEthRate;

            // 4. Create the main Order record
            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = fiatTotal,
                TotalEthAmount = ethTotal,
                TransactionHash = transactionHash,
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = newOrder.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product!.Price
                };
                _context.OrderItems.Add(orderItem);


                if (item.Product != null)
                {
                    item.Product.StockQuantity -= item.Quantity;
                }
            }

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}