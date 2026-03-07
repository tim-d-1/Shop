using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;

namespace InternetShop.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetOrSetGuestId()
        {
            if (Request.Cookies.TryGetValue("GuestId", out string? guestId)) return guestId;

            guestId = Guid.NewGuid().ToString();
            Response.Cookies.Append("GuestId", guestId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
            return guestId;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetOrSetGuestId();

            if (userId != null)
            {
                Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
        }
    }
}