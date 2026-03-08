using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;
using InternetShop.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InternetShop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ProductCategory { get; set; }

        public SelectList? Categories { get; set; }
        public async Task OnGetAsync()
        {
            IQueryable<string> categoryQuery = from p in _context.Products
                                               orderby p.Category
                                               select p.Category;
            var products = from p in _context.Products
                           select p;

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.Name.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(ProductCategory))
            {
                products = products.Where(x => x.Category == ProductCategory);
            }

            Categories = new SelectList(await categoryQuery.Distinct().ToListAsync());
            Products = await products.ToListAsync();
        }

        private string GetOrSetGuestId()
        {
            if (Request.Cookies.TryGetValue("GuestId", out string? guestId)) return guestId;

            guestId = Guid.NewGuid().ToString();
            Response.Cookies.Append("GuestId", guestId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
            return guestId;
        }

        public async Task<bool> IsInFavorites(int productId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetOrSetGuestId();

            if (userId is null) return false;

            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            if (existingItem == null) return false;

            return true;
        }
    }
}