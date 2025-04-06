using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using webb_tst_site.Data;
using webb_tst_site.Models; // Добавлен недостающий using

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23))));

// Добавьте в начало:
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFile("Logs/myapp-{Date}.txt", LogLevel.Warning);

// Настройка cookie для разработки
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "RuneRandomizer.Auth";
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});


// Add logging
builder.Services.AddLogging(configure =>
    configure.AddConsole().AddDebug());

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        // Seed initial admin if no users exist
        if (!context.Users.Any())
        {
            context.Users.Add(new User // Убрано Models.User, так как using уже добавлен
            {
                Username = "admin",
                PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // sha256("admin123")
                Role = "Admin"
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Middleware для обработки Telegram WebApp данных
app.Use(async (context, next) =>
{
    if (context.Request.Query.TryGetValue("tgWebAppData", out var tgData))
    {
        try
        {
            // Временная заглушка для ParseTelegramData - нужно реализовать
            var userData = new TelegramUserData
            {
                Username = tgData.ToString().Split('=')[1].Split('&')[0],
                IsAdmin = true // Временное значение для теста
            };

            context.Items["TelegramUser"] = userData;

            // Автоматическая авторизация для Telegram пользователей
            if (userData.IsAdmin)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userData.Username),
                    new(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "TelegramAuth");
                context.User = new ClaimsPrincipal(claimsIdentity);
            }
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error parsing Telegram WebApp data");
        }
    }
    await next();
});

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

/*app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});*/

// Создание папок для загрузки, если их нет
var uploadsFolder = Path.Combine(builder.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsFolder))
{
    Directory.CreateDirectory(uploadsFolder);
    Directory.CreateDirectory(Path.Combine(uploadsFolder, "runes"));
    Directory.CreateDirectory(Path.Combine(uploadsFolder, "spheres"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.Always
});
app.Run();

// Добавлен класс для хранения данных пользователя Telegram
public class TelegramUserData
{
    public string Username { get; set; }
    public bool IsAdmin { get; set; }
}