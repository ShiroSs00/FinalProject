using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RazorPage_test.Pages.Payment;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly ITransactionService _transactionService;

    public CheckoutModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [BindProperty]
    public string Type { get; set; } = string.Empty;

    [BindProperty]
    public decimal Amount { get; set; }

    public IActionResult OnGet(string type, decimal amount)
    {
        if (string.IsNullOrEmpty(type) || amount <= 0)
        {
            return RedirectToPage("/Payment/Index");
        }

        Type = type;
        Amount = amount;
        return Page();
    }

    public IActionResult OnPost()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out int userId))
        {
            var result = _transactionService.ProcessPayment(userId, Amount, Type);
            if (result.Success)
            {
                TempData["Success"] = $"Thanh toán thành công! {(Type == "Points" ? $"Bạn đã nhận được {Amount:N0} điểm." : "Bạn đã trở thành VIP!")}";
                return RedirectToPage("/Dashboard");
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

        return Page();
    }
}
