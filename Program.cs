using GoCylone.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GoCyloneDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add distributed memory cache for sessions
builder.Services.AddDistributedMemoryCache();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GoCyloneDbContext>();

    try
    {
        // Apply pending migrations
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add session middleware BEFORE authorization
app.UseSession();

// Add custom authentication middleware
app.Use(async (context, next) =>
{
    var userId = context.Session.GetString("UserId");
    var path = context.Request.Path.Value?.ToLower() ?? "";

    // Public paths that don't require authentication
    var publicPaths = new[] { "/login", "/register", "/auth/login", "/auth/register", "/clearcookies" };
    var isPublicPath = publicPaths.Any(p => path.StartsWith(p));
    var isStaticFile = path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") || path.StartsWith("/images");

    // Redirect unauthenticated users to login (except for public paths and static files)
    if (string.IsNullOrEmpty(userId) && !isPublicPath && !isStaticFile && path != "/")
    {
        context.Response.Redirect("/Login");
        return;
    }

    // Redirect root to login if not authenticated
    if (path == "/" && string.IsNullOrEmpty(userId))
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

