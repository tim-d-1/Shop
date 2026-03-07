using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;

namespace InternetShop.Pages.Favorites
{
    public class RemoveModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public RemoveModel(ApplicationDbContext context) => _context = context;

        private string GetOrSetGuestId()
        {
            if (Request.Cookies.TryGetValue("GuestId", out string? guestId)) return guestId;
            guestId = Guid.NewGuid().ToString();
            Response.Cookies.Append("GuestId", guestId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
            return guestId;
        }

        public async Task<IActionResult> OnPostAsync(int? id, string? returnUrl)
        {
            if (id == null) return NotFound();

            // Support Guest ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetOrSetGuestId();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == id && w.UserId == userId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }
    }
}