using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InternetShop.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. SWAP: Change from UseSqlite to UseSqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. IDENTITY SETUP: 
// Set RequireConfirmedAccount to false so you can actually log in during development 
// without setting up an SMTP/Email server.
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// 4. ADD CUSTOM SERVICES:
// Register an HttpClient for your CoinGecko integration later
builder.Services.AddHttpClient();

var app = builder.Build();

// 5. DATABASE AUTO-MIGRATION (Optional but handy for Docker)
// This ensures the DB is updated whenever the app starts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // db.Database.Migrate(); // Uncomment this if you want auto-migrations on startup
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Note: Replaced MapStaticAssets for standard Bootstrap support

app.UseRouting();

app.UseAuthentication(); // Must come before Authorization
app.UseAuthorization();

app.MapRazorPages();

app.Run();