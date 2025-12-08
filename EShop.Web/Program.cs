using EShop.BLL.Services;
using EShop.DAL;
using EShop.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using EShop.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Add services to the container.
// ==========================================

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giỏ hàng tồn tại 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<EShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// 2. Dependency Injection (DI) Configuration
// ==========================================

// Đăng ký Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
// ==========================================
// 3. Authentication Configuration (Cookie)
// ==========================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; 
        options.AccessDeniedPath = "/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); 
    });

var app = builder.Build();

// ==========================================
// 4. Configure the HTTP request pipeline.
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.MapHub<ECommerceHub>("/ecommerceHub");

// ==========================================
// 5. Automatic Database Update (Fix for Missing IsDeleted Column)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EShopDbContext>();

        // SQL to check if IsDeleted column exists, and if not, add it.
        var sql = @"
            IF NOT EXISTS (
                SELECT * FROM sys.columns
                WHERE Name = N'IsDeleted'
                AND Object_ID = Object_ID(N'Products')
            )
            BEGIN
                ALTER TABLE Products ADD IsDeleted BIT NOT NULL DEFAULT 0
            END";

        context.Database.ExecuteSqlRaw(sql);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while updating the database schema.");
    }
}

app.Run();