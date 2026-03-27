using DAL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_test.Pages.AdminDashboard;

public class UsersModel : PageModel
{
    private readonly IUserService _userService;

    public UsersModel(IUserService userService)
    {
        _userService = userService;
    }

    public List<User> Users { get; set; } = new();

    public void OnGet()
    {
        Users = _userService.GetAllUsers();
    }
}
