using BLL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage_test.Hubs;
using System.Security.Claims;

namespace RazorPage_test.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly INutritionService _nutritionService;
    private readonly IRecommendedMealService _mealService;
    private readonly IDailyRecordService _dailyRecordService;
    private readonly IMealReviewService _reviewService;
    private readonly IUserService _userService;
    private readonly IHubContext<RecipeHub> _hubContext;

    public DashboardModel(
        INutritionService nutritionService, 
        IRecommendedMealService mealService, 
        IDailyRecordService dailyRecordService,
        IMealReviewService reviewService,
        IUserService userService,
        IHubContext<RecipeHub> hubContext)
    {
        _nutritionService = nutritionService;
        _mealService = mealService;
        _dailyRecordService = dailyRecordService;
        _reviewService = reviewService;
        _userService = userService;
        _hubContext = hubContext;
    }

    public string UserInitial { get; set; } = "U";
    public string UserName { get; set; } = "Thành viên";
    
    public BLL.DTOs.UserProfileDto? Profile { get; set; }
    public DAL.Models.DailyRecord TodayRecord { get; set; } = new();
    
    public int UserPoints { get; set; }
    
    // For Chart.js
    public List<DAL.Models.DailyRecord> Last7Days { get; set; } = new();

    [BindProperty]
    public int CaloriesToAdd { get; set; }
    
    // Data list
    public List<DAL.Models.RecommendedMeal> RecommendedMeals { get; set; } = new();

    public void OnGet()
    {
        UserName = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "Người dùng";
        UserInitial = UserName.Length > 0 ? UserName[0].ToString().ToUpper() : "U";

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out int userId))
        {
            Profile = _nutritionService.GetProfile(userId);
            TodayRecord = _dailyRecordService.GetTodayRecord(userId);
            Last7Days = _dailyRecordService.GetLast7Days(userId);
            
            var user = _userService.GetById(userId);
            UserPoints = user?.Points ?? 0;
        }

        RecommendedMeals = _mealService.GetAll();
    }

    public IActionResult OnPostLogCalories()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out int userId) && CaloriesToAdd > 0)
        {
            _dailyRecordService.LogCalories(userId, CaloriesToAdd);
        }
        return RedirectToPage();
    }

    // Bỏ hàm OnPostSubmitReview cũ hoặc giữ lại tuỳ ý (em thay bằng Ajax)
    
    // --- Tính năng Real-time Reviews (AJAX) --- //

    public IActionResult OnGetLoadReviewsAjax(string mealName)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int currentUserId = int.TryParse(userIdClaim, out int uid) ? uid : 0;
        
        var reviews = _reviewService.GetReviewsByMeal(mealName, currentUserId);
        return new JsonResult(new { success = true, data = reviews });
    }

    public async Task<IActionResult> OnPostSubmitReviewAjax([FromBody] SubmitReviewRequest req)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out int userId) && !string.IsNullOrEmpty(req.MealName))
        {
            var reviewDto = _reviewService.SubmitReview(userId, req.MealName, req.Rating, req.Comment);
            
            // Broadcast via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNewReview", req.MealName, reviewDto);
            
            return new JsonResult(new { success = true, data = reviewDto, message = "Bạn nhận được +10 điểm thưởng! 🎁" });
        }
        return new JsonResult(new { success = false, message = "Lỗi xác thực người dùng." });
    }

    public async Task<IActionResult> OnPostEditReviewAjax([FromBody] EditReviewRequest req)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out int userId))
        {
            var updated = _reviewService.EditReview(req.Id, userId, req.Rating, req.Comment);
            if (updated != null)
            {
                await _hubContext.Clients.All.SendAsync("ReviewUpdated", updated.MealName, updated);
                return new JsonResult(new { success = true, data = updated });
            }
        }
        return new JsonResult(new { success = false, message = "Không thể sửa đánh giá này." });
    }

    public async Task<IActionResult> OnPostDeleteReviewAjax([FromBody] DeleteReviewRequest req)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out int userId))
        {
            bool deleted = _reviewService.DeleteReview(req.Id, userId);
            if (deleted)
            {
                await _hubContext.Clients.All.SendAsync("ReviewDeleted", req.MealName, req.Id);
                return new JsonResult(new { success = true, id = req.Id });
            }
        }
        return new JsonResult(new { success = false, message = "Không thể xóa đánh giá này." });
    }

    // Các class request model cho AJAX
    public class SubmitReviewRequest { public string MealName { get; set; } = string.Empty; public int Rating { get; set; } public string Comment { get; set; } = string.Empty; }
    public class EditReviewRequest { public int Id { get; set; } public int Rating { get; set; } public string Comment { get; set; } = string.Empty; }
    public class DeleteReviewRequest { public int Id { get; set; } public string MealName { get; set; } = string.Empty; }


    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }
}
