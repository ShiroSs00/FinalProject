using DAL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace RazorPage_test.Pages.AdminDashboard.Gifts;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IGiftService _giftService;

    public IndexModel(IGiftService giftService)
    {
        _giftService = giftService;
    }

    public List<Gift> Gifts { get; set; } = new();

    public void OnGet()
    {
        Gifts = _giftService.GetAllGifts();
    }
}
