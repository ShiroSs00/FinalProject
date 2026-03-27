using DAL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace RazorPage_test.Pages.AdminDashboard.Gifts;

[Authorize]
public class EditModel : PageModel
{
    private readonly IGiftService _giftService;

    public EditModel(IGiftService giftService)
    {
        _giftService = giftService;
    }

    [BindProperty]
    public Gift Gift { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        var existing = _giftService.GetGiftById(id);
        if (existing == null) return NotFound();

        Gift = existing;
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        _giftService.UpdateGift(Gift);
        TempData["Success"] = $"Đã cập nhật phần quà {Gift.Name} thành công!";
        return RedirectToPage("Index");
    }
}
