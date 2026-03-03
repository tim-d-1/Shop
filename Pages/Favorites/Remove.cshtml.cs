using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;

namespace InternetShop.Pages.Favorites
{
    [Authorize]
    public class RemoveModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RemoveModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == id && w.UserId == userId);

            if (existingItem != null)
            {
                _context.WishlistItems.Remove(existingItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}