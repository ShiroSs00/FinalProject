using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RazorPage_test.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly INutritionService _nutritionService;

    public ProfileModel(INutritionService nutritionService)
    {
        _nutritionService = nutritionService;
    }

    public UserProfileDto Profile { get; set; } = new();
    public bool Saved { get; set; }

    public string BmiClass => Profile.BmiCategory switch
    {
        "Thieu can"    => "bmi-thin",
        "Binh thuong"  => "bmi-normal",
        "Thua can"     => "bmi-over",
        _              => "bmi-obese"
    };

    // Convert BMI 10-40 -> 0-100% for pointer
    public double BmiPointerLeft => Math.Clamp((Profile.Bmi - 10.0) / 30.0 * 100.0, 2, 98);

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    public void OnGet()
    {
        var existing = _nutritionService.GetProfile(GetUserId());
        if (existing != null)
            Profile = existing;
    }

    [BindProperty] public int Age { get; set; }
    [BindProperty] public double Height { get; set; }
    [BindProperty] public double Weight { get; set; }
    [BindProperty] public string Gender { get; set; } = "Nam";
    [BindProperty] public string ActivityLevel { get; set; } = "Sedentary";
    [BindProperty] public string Goal { get; set; } = "DuyTri";

    public IActionResult OnPost()
    {
        var dto = new UserProfileDto
        {
            UserId = GetUserId(),
            Age = Age,
            Height = Height,
            Weight = Weight,
            Gender = Gender,
            ActivityLevel = ActivityLevel,
            Goal = Goal
        };

        _nutritionService.SaveProfile(dto);
        Profile = _nutritionService.Calculate(dto);
        Saved = true;
        return Page();
    }
}
