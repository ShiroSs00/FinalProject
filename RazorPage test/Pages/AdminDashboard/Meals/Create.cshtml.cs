using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using RazorPage_test.Services;

namespace RazorPage_test.Pages.AdminDashboard;

public class CreateModel : PageModel
{
    private readonly IRecommendedMealService _mealService;
    private readonly IMenuBroadcastService _broadcastService;

    public CreateModel(IRecommendedMealService mealService, IMenuBroadcastService broadcastService)
    {
        _mealService = mealService;
        _broadcastService = broadcastService;
    }

    [BindProperty]
    public RecommendedMeal Meal { get; set; } = new();

    public List<string> AvailableTypes { get; set; } = new();

    public void OnGet()
    {
        AvailableTypes = _mealService.GetAvailableMealTypes();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AvailableTypes = _mealService.GetAvailableMealTypes();

        if (!ModelState.IsValid)
            return Page();

        if (!AvailableTypes.Contains(Meal.MealType))
        {
            ModelState.AddModelError("", "Loại bữa này đã tồn tại.");
            return Page();
        }

        _mealService.Create(Meal);
        
        // Tự động phát sóng sau khi tạo mới
        await _broadcastService.BroadcastMenuAsync();
        
        TempData["Success"] = $"Đã thêm thành công món cho cữ {Meal.MealType} và cập nhật tới Dashboard!";
        return RedirectToPage("Index");
    }
}
