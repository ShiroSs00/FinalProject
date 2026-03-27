using BLL.Services;
using DAL.Data;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RazorPage_test.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ================================
// Database – EF Core + SQL Server
// ================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ================================
// Dependency Injection (3-Layer)
// ================================
// DAL
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IRecommendedMealRepository, RecommendedMealRepository>();
builder.Services.AddScoped<IDailyRecordRepository, DailyRecordRepository>();
builder.Services.AddScoped<IMealReviewRepository, MealReviewRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IUserGiftRepository, UserGiftRepository>();

// BLL
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IAiChatService, GroqChatService>();
builder.Services.AddScoped<IRecommendedMealService, RecommendedMealService>();
builder.Services.AddScoped<IDailyRecordService, DailyRecordService>();
builder.Services.AddScoped<IMealReviewService, MealReviewService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IGiftService, GiftService>();

// Web Services
builder.Services.AddScoped<RazorPage_test.Services.IMenuBroadcastService, RazorPage_test.Services.MenuBroadcastService>();

// HttpClient for Groq API
builder.Services.AddHttpClient("Groq");

// Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/AdminDashboard", "RequireAdminRole");
});

// SignalR
builder.Services.AddSignalR();

// Session (dùng cho TempData và lịch sử chat)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// ================================
// Auto-migrate database on startup
// ================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<RecipeHub>("/recipeHub");

app.MapGet("/", context =>
{
    context.Response.Redirect("/Dashboard");
    return Task.CompletedTask;
});

app.Run();
