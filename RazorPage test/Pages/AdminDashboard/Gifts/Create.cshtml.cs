using DAL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace RazorPage_test.Pages.AdminDashboard.Gifts;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IGiftService _giftService;

    public CreateModel(IGiftService giftService)
    {
        _giftService = giftService;
    }

    [BindProperty]
    public Gift Gift { get; set; } = new() { IsActive = true, Stock = -1 };

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        _giftService.AddGift(Gift);
        TempData["Success"] = $"Đã tạo phần quà {Gift.Name} thành công!";
        return RedirectToPage("Index");
    }
}
