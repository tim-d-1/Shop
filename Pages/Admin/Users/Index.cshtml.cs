using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Pages.Admin.Users
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserViewModel
        {
            public required string Id { get; set; }
            public required string Email { get; set; }
            public IList<string> Roles { get; set; } = new List<string>();
        }

        public List<UserViewModel> UsersList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "No Email",
                    Roles = roles
                });
            }
        }
        public async Task<IActionResult> OnPostPromoteAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDemoteAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var admin = await _userManager.FindByIdAsync(id);
            if (admin != null)
            {
                if (await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(admin, "Admin");
                }
            }

            return RedirectToPage();
        }
    }
}