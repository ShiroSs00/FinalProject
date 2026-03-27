using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using RazorPage_test.Services;

namespace RazorPage_test.Pages.AdminDashboard;

public class EditModel : PageModel
{
    private readonly IRecommendedMealService _mealService;
    private readonly IMenuBroadcastService _broadcastService;

    public EditModel(IRecommendedMealService mealService, IMenuBroadcastService broadcastService)
    {
        _mealService = mealService;
        _broadcastService = broadcastService;
    }

    [BindProperty]
    public RecommendedMeal Meal { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        var existing = _mealService.GetById(id);
        if (existing == null) return NotFound();

        Meal = existing;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _mealService.Update(Meal);
        
        // Tự động phát sóng sau khi sửa
        await _broadcastService.BroadcastMenuAsync();
        
        TempData["Success"] = "Đã cập nhật món ăn thành công và cập nhật tới Dashboard!";
        return RedirectToPage("Index");
    }
}
