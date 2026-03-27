using BLL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RazorPage_test.Pages.Payment;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;

    public CheckoutModel(ITransactionService transactionService, IUserService userService)
    {
        _transactionService = transactionService;
        _userService = userService;
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

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out int userId))
        {
            var result = _transactionService.ProcessPayment(userId, Amount, Type);
            if (result.Success)
            {
                // Refresh the authentication cookie so the new Role is reflected
                // in User.IsInRole() immediately (e.g. VIP access to AI Chat).
                var updatedUser = _userService.GetById(userId);
                if (updatedUser != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, updatedUser.Id.ToString()),
                        new Claim(ClaimTypes.Name, updatedUser.Username),
                        new Claim(ClaimTypes.Email, updatedUser.Email),
                        new Claim(ClaimTypes.Role, updatedUser.Role),
                        new Claim("FullName", updatedUser.FullName)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }

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
