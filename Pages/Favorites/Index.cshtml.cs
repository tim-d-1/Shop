using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;

namespace InternetShop.Pages.Favorites
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

        public IList<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetOrSetGuestId();

            if (userId != null)
            {
                WishlistItems = await _context.WishlistItems
                    .Include(w => w.Product)
                    .Where(w => w.UserId == userId)
                    .ToListAsync();
            }
        }
    }
}