using BLL.DTOs;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage_test.Hubs;
using RazorPage_test.Services;

namespace RazorPage_test.Pages.AdminDashboard;

public class IndexModel : PageModel
{
    private readonly IRecommendedMealService _mealService;
    private readonly IMenuBroadcastService _broadcastService;

    public IndexModel(IRecommendedMealService mealService, IMenuBroadcastService broadcastService)
    {
        _mealService = mealService;
        _broadcastService = broadcastService;
    }

    public List<RecommendedMeal> Meals { get; set; } = new();
    public bool CanCreate { get; set; }

    public void OnGet()
    {
        Meals = _mealService.GetAll();
        CanCreate = _mealService.GetAvailableMealTypes().Any();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        _mealService.Delete(id);
        
        // Tự động phát sóng sau khi xóa
        await _broadcastService.BroadcastMenuAsync();
        
        TempData["Success"] = "Đã xóa 1 cữ ăn thành công và cập nhật tới Dashboard!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPushSignalRAsync()
    {
        await _broadcastService.BroadcastMenuAsync();
        TempData["Success"] = "Đã phát sóng thực đơn mới nhất tới Dashboard người dùng!";
        return RedirectToPage();
    }
}
