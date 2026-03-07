using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;

namespace InternetShop.Pages.Favorites
{
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetOrSetGuestId();
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == id && w.UserId == userId);

            if (existingItem == null)
            {
                var newItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = id.Value
                };
                _context.WishlistItems.Add(newItem);
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { success = true });
        }
    }
}