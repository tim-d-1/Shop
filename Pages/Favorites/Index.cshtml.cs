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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                WishlistItems = await _context.WishlistItems
                    .Include(w => w.Product)
                    .Where(w => w.UserId == userId)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}