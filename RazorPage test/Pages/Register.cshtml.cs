using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_test.Pages;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        var (success, error) = _userService.Register(new RegisterDto
        {
            Username = Username,
            Password = Password,
            Email = Email,
            FullName = FullName
        });

        if (!success)
        {
            ErrorMessage = error;
            return Page();
        }

        TempData["RegisterSuccess"] = $"Đăng ký thành công! Chào mừng {FullName}. Vui lòng đăng nhập.";
        return RedirectToPage("/Login");
    }
}
