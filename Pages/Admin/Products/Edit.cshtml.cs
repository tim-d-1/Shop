using InternetShop.Data;
using InternetShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetShop.Pages_Admin_Products
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly InternetShop.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(InternetShop.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public IFormFile? UploadedImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var productToUpdate = await _context.Products.FindAsync(Product.Id);
            if (productToUpdate == null) return NotFound();

            if (UploadedImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + UploadedImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedImage.CopyToAsync(fileStream);
                }

                // Optional: Delete the old image file here if you want to save space

                productToUpdate.ImageUrl = "/images/products/" + uniqueFileName;
            }

            // Update other properties
            productToUpdate.Name = Product.Name;
            productToUpdate.Description = Product.Description;
            productToUpdate.Price = Product.Price;
            productToUpdate.StockQuantity = Product.StockQuantity;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
