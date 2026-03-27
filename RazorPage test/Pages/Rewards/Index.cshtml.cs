using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RazorPage_test.Pages.Rewards;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IGiftService _giftService;
    private readonly IUserService _userService;

    public IndexModel(IGiftService giftService, IUserService userService)
    {
        _giftService = giftService;
        _userService = userService;
    }

    public List<Gift> Gifts { get; set; } = new();
    public int UserPoints { get; set; }

    public void OnGet()
    {
        Gifts = _giftService.GetAllActiveGifts();

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out int userId))
        {
            var user = _userService.GetById(userId);
            UserPoints = user?.Points ?? 0;
        }
    }

    public IActionResult OnPostExchange(int giftId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out int userId))
        {
            var result = _giftService.ExchangeGift(userId, giftId);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        else
        {
            TempData["Error"] = "Lỗi xác thực người dùng.";
        }

        return RedirectToPage();
    }
}
