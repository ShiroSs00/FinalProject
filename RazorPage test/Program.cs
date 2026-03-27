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

    // ================================
    // Seed mock data (runs once – skips if Users already exist)
    // ================================
    if (!db.Users.Any())
    {
        SeedData(db);
    }
}

static void SeedData(AppDbContext db)
{
    var rng = new Random(42); // fixed seed for reproducible data

    // ── 1. Users ──────────────────────────────────────────────
    var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

    var users = new List<DAL.Models.User>
    {
        new() { Username = "admin",      PasswordHash = passwordHash, Email = "admin@nutrifood.vn",      FullName = "Quản Trị Viên",  Role = "Admin", Points = 500,  CreatedAt = new DateTime(2025, 1, 10, 8, 0, 0, DateTimeKind.Utc) },
        new() { Username = "vipuser",    PasswordHash = passwordHash, Email = "vip@nutrifood.vn",        FullName = "Nguyễn Văn VIP", Role = "VIP",   Points = 320,  CreatedAt = new DateTime(2025, 2, 14, 10, 0, 0, DateTimeKind.Utc) },
        new() { Username = "hoanganh",   PasswordHash = passwordHash, Email = "hoanganh@gmail.com",      FullName = "Trần Hoàng Anh", Role = "User",  Points = 150,  CreatedAt = new DateTime(2025, 3, 5, 9, 0, 0, DateTimeKind.Utc) },
        new() { Username = "minhthao",   PasswordHash = passwordHash, Email = "minhthao@gmail.com",      FullName = "Lê Minh Thảo",   Role = "User",  Points = 80,   CreatedAt = new DateTime(2025, 5, 20, 14, 0, 0, DateTimeKind.Utc) },
        new() { Username = "ducpham",    PasswordHash = passwordHash, Email = "ducpham@yahoo.com",       FullName = "Phạm Đức",       Role = "User",  Points = 200,  CreatedAt = new DateTime(2025, 7, 1, 7, 0, 0, DateTimeKind.Utc) },
        new() { Username = "thuylinh",   PasswordHash = passwordHash, Email = "thuylinh@outlook.com",    FullName = "Võ Thuỳ Linh",   Role = "VIP",   Points = 450,  CreatedAt = new DateTime(2025, 9, 15, 16, 0, 0, DateTimeKind.Utc) },
        new() { Username = "quanho",     PasswordHash = passwordHash, Email = "quanho@gmail.com",        FullName = "Hồ Trần Quân",   Role = "User",  Points = 60,   CreatedAt = new DateTime(2025, 11, 3, 11, 0, 0, DateTimeKind.Utc) },
        new() { Username = "ngochan",    PasswordHash = passwordHash, Email = "ngochan@gmail.com",       FullName = "Bùi Ngọc Hân",   Role = "User",  Points = 110,  CreatedAt = new DateTime(2026, 1, 8, 8, 30, 0, DateTimeKind.Utc) },
    };
    db.Users.AddRange(users);
    db.SaveChanges();

    // ── 2. User Profiles ──────────────────────────────────────
    var profiles = new List<DAL.Models.UserProfile>
    {
        new() { UserId = users[0].Id, Age = 30, Height = 175, Weight = 72, Gender = "Nam", ActivityLevel = "ModeratelyActive", Goal = "DuyTri" },
        new() { UserId = users[1].Id, Age = 26, Height = 168, Weight = 65, Gender = "Nam", ActivityLevel = "VeryActive",       Goal = "TangCo" },
        new() { UserId = users[2].Id, Age = 22, Height = 170, Weight = 78, Gender = "Nam", ActivityLevel = "LightlyActive",    Goal = "GiamCan" },
        new() { UserId = users[3].Id, Age = 28, Height = 160, Weight = 55, Gender = "Nu",  ActivityLevel = "ModeratelyActive", Goal = "DuyTri" },
        new() { UserId = users[4].Id, Age = 35, Height = 180, Weight = 85, Gender = "Nam", ActivityLevel = "Sedentary",        Goal = "GiamCan" },
        new() { UserId = users[5].Id, Age = 24, Height = 163, Weight = 52, Gender = "Nu",  ActivityLevel = "VeryActive",       Goal = "TangCo" },
        new() { UserId = users[6].Id, Age = 21, Height = 172, Weight = 68, Gender = "Nam", ActivityLevel = "LightlyActive",    Goal = "DuyTri" },
        new() { UserId = users[7].Id, Age = 27, Height = 158, Weight = 50, Gender = "Nu",  ActivityLevel = "ModeratelyActive", Goal = "GiamCan" },
    };
    db.UserProfiles.AddRange(profiles);
    db.SaveChanges();

    // ── 3. Recommended Meals ──────────────────────────────────
    var meals = new List<DAL.Models.RecommendedMeal>
    {
        new() { MealType = "Breakfast", Name = "Phở Bò",            Description = "Phở bò truyền thống với nước dùng ninh xương 12 tiếng, thịt bò tái lăn",                 Calories = 450, Minutes = 15, Icon = "🍜" },
        new() { MealType = "Lunch",     Name = "Cơm Gà Xối Mỡ",    Description = "Cơm trắng dẻo kèm gà xối mỡ giòn rụm, rau luộc và canh chua",                           Calories = 650, Minutes = 20, Icon = "🍗" },
        new() { MealType = "Dinner",    Name = "Bún Chả Hà Nội",    Description = "Bún chả thơm lừng với chả viên nướng than hoa, nước mắm chua ngọt và rau sống",           Calories = 520, Minutes = 25, Icon = "🥗" },
    };
    db.RecommendedMeals.AddRange(meals);
    db.SaveChanges();

    // ── 4. DailyRecords (Jan 2025 → today) ────────────────────
    var dailyRecords = new List<DAL.Models.DailyRecord>();
    var startDate = new DateTime(2025, 1, 1);
    var endDate = DateTime.Now.Date;

    foreach (var user in users)
    {
        int baseCal = rng.Next(1600, 2200);
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // ~70 % chance of having a record on any given day
            if (rng.NextDouble() > 0.30)
            {
                dailyRecords.Add(new DAL.Models.DailyRecord
                {
                    UserId = user.Id,
                    Date = date,
                    CaloriesConsumed = baseCal + rng.Next(-400, 401)
                });
            }
        }
    }
    db.DailyRecords.AddRange(dailyRecords);
    db.SaveChanges();

    // ── 5. Transactions (2025 – 2026) ─────────────────────────
    var txTypes = new[] { "VIP", "Points" };
    var transactions = new List<DAL.Models.Transaction>();

    // Spread transactions across the date range
    for (var month = new DateTime(2025, 1, 1); month <= DateTime.UtcNow; month = month.AddMonths(1))
    {
        int txCount = rng.Next(3, 8);
        for (int t = 0; t < txCount; t++)
        {
            var user = users[rng.Next(users.Count)];
            var type = txTypes[rng.Next(txTypes.Length)];
            var day = rng.Next(1, DateTime.DaysInMonth(month.Year, month.Month) + 1);
            var txDate = new DateTime(month.Year, month.Month, day, rng.Next(7, 22), rng.Next(0, 60), 0, DateTimeKind.Utc);
            if (txDate > DateTime.UtcNow) continue;

            transactions.Add(new DAL.Models.Transaction
            {
                UserId = user.Id,
                Amount = type == "VIP" ? 99000m : rng.Next(1, 6) * 50000m,
                Type = type,
                Status = "Completed",
                Description = type == "VIP" ? "Đăng ký gói VIP" : $"Nạp điểm thưởng",
                CreatedAt = txDate
            });
        }
    }

    // Ensure some transactions in the last 7 days for the admin chart
    for (int i = 0; i < 7; i++)
    {
        var day = DateTime.UtcNow.AddDays(-i);
        int count = rng.Next(1, 4);
        for (int c = 0; c < count; c++)
        {
            var user = users[rng.Next(users.Count)];
            var type = txTypes[rng.Next(txTypes.Length)];
            transactions.Add(new DAL.Models.Transaction
            {
                UserId = user.Id,
                Amount = type == "VIP" ? 99000m : rng.Next(1, 6) * 50000m,
                Type = type,
                Status = "Completed",
                Description = type == "VIP" ? "Đăng ký gói VIP" : "Nạp điểm thưởng",
                CreatedAt = new DateTime(day.Year, day.Month, day.Day, rng.Next(7, 22), rng.Next(0, 60), 0, DateTimeKind.Utc)
            });
        }
    }

    db.Transactions.AddRange(transactions);
    db.SaveChanges();

    // ── 6. Gifts (Rewards Store) ──────────────────────────────
    var gifts = new List<DAL.Models.Gift>
    {
        new() { Name = "Voucher Grab Food 50K",      Description = "Mã giảm giá 50.000đ cho đơn Grab Food",                          PointsRequired = 100,  Icon = "🎫", Stock = 50,  IsActive = true,  CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Bình Nước Giữ Nhiệt",        Description = "Bình giữ nhiệt inox 500ml in logo NutriFood",                    PointsRequired = 250,  Icon = "🧴", Stock = 30,  IsActive = true,  CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Túi Tote NutriFood",          Description = "Túi vải canvas thiết kế exclusive, thân thiện môi trường",        PointsRequired = 150,  Icon = "👜", Stock = 100, IsActive = true,  CreatedAt = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Sách '100 Món Ăn Healthy'",   Description = "Sách công thức nấu ăn lành mạnh, bìa cứng, 200 trang",           PointsRequired = 300,  Icon = "📖", Stock = 20,  IsActive = true,  CreatedAt = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Cân Điện Tử Thông Minh",      Description = "Cân sức khoẻ kết nối Bluetooth, đo 13 chỉ số cơ thể",            PointsRequired = 500,  Icon = "⚖️",  Stock = 10,  IsActive = true,  CreatedAt = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Voucher Gym 1 Tháng",         Description = "Thẻ tập gym 1 tháng tại hệ thống California Fitness",             PointsRequired = 800,  Icon = "💪", Stock = 5,   IsActive = true,  CreatedAt = new DateTime(2025, 6, 20, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Hộp Meal Prep 3 Ngăn (x5)",   Description = "Bộ 5 hộp thuỷ tinh chia ngăn, an toàn lò vi sóng",               PointsRequired = 200,  Icon = "🍱", Stock = 40,  IsActive = true,  CreatedAt = new DateTime(2025, 8, 5, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Voucher ShopeeFood 100K",     Description = "Mã giảm giá 100.000đ cho ShopeeFood",                            PointsRequired = 180,  Icon = "🛒", Stock = -1,  IsActive = true,  CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Áo Thun NutriFood Limited",   Description = "Áo thun cotton 100% phiên bản giới hạn, size S-XL",              PointsRequired = 350,  Icon = "👕", Stock = 25,  IsActive = true,  CreatedAt = new DateTime(2025, 11, 11, 0, 0, 0, DateTimeKind.Utc) },
        new() { Name = "Máy Xay Sinh Tố Mini",        Description = "Máy xay sinh tố cầm tay USB, 380ml, tiện mang theo",             PointsRequired = 600,  Icon = "🥤", Stock = 15,  IsActive = true,  CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    };
    db.Gifts.AddRange(gifts);
    db.SaveChanges();

    // ── 7. Meal Reviews ───────────────────────────────────────
    var mealNames = new[] { "Phở Bò", "Cơm Gà Xối Mỡ", "Bún Chả Hà Nội" };
    var sampleComments = new[]
    {
        "Ngon lắm, sẽ ăn lại!",
        "Vừa miệng, healthy nữa 👍",
        "Hơi nhiều dầu mỡ nhưng vị ổn",
        "Tuyệt vời! Đúng khẩu vị của mình",
        "Bình thường, không có gì đặc biệt",
        "Rất tốt cho sức khoẻ, recommend!",
        "Ngon nhưng hơi ít thịt",
        "Phần ăn vừa đủ, giá hợp lý",
        "Mình thích món này, 10 điểm!",
        "Lần sau sẽ order thêm",
    };

    var reviews = new List<DAL.Models.MealReview>();
    foreach (var user in users)
    {
        foreach (var mealName in mealNames)
        {
            if (rng.NextDouble() > 0.35) // ~65 % chance
            {
                var reviewDate = user.CreatedAt.AddDays(rng.Next(1, 200));
                if (reviewDate > DateTime.UtcNow) reviewDate = DateTime.UtcNow.AddDays(-rng.Next(1, 30));

                reviews.Add(new DAL.Models.MealReview
                {
                    UserId = user.Id,
                    MealName = mealName,
                    Rating = rng.Next(3, 6), // 3-5 stars
                    Comment = sampleComments[rng.Next(sampleComments.Length)],
                    CreatedAt = reviewDate
                });
            }
        }
    }
    db.MealReviews.AddRange(reviews);
    db.SaveChanges();

    // ── 8. UserGifts (exchange history) ───────────────────────
    var userGifts = new List<DAL.Models.UserGift>();
    foreach (var user in users.Where(u => u.Points >= 100))
    {
        int exchanges = rng.Next(1, 3);
        for (int e = 0; e < exchanges; e++)
        {
            var gift = gifts[rng.Next(gifts.Count)];
            userGifts.Add(new DAL.Models.UserGift
            {
                UserId = user.Id,
                GiftId = gift.Id,
                ExchangedAt = user.CreatedAt.AddDays(rng.Next(30, 300))
            });
        }
    }
    db.UserGifts.AddRange(userGifts);
    db.SaveChanges();
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
