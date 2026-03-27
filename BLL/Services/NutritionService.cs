using BLL.DTOs;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public interface INutritionService
{
    UserProfileDto? GetProfile(int userId);
    void SaveProfile(UserProfileDto dto);
    UserProfileDto Calculate(UserProfileDto dto);
}

public class NutritionService : INutritionService
{
    private readonly IUserProfileRepository _repo;

    public NutritionService(IUserProfileRepository repo)
    {
        _repo = repo;
    }

    public UserProfileDto? GetProfile(int userId)
    {
        var entity = _repo.GetByUserId(userId);
        if (entity == null) return null;
        var dto = MapToDto(entity);
        return Calculate(dto);
    }

    public void SaveProfile(UserProfileDto dto)
    {
        var entity = new UserProfile
        {
            UserId = dto.UserId,
            Age = dto.Age,
            Height = dto.Height,
            Weight = dto.Weight,
            Gender = dto.Gender,
            ActivityLevel = dto.ActivityLevel,
            Goal = dto.Goal
        };
        _repo.Save(entity);
    }

    public UserProfileDto Calculate(UserProfileDto dto)
    {
        // BMI = weight(kg) / (height(m))^2
        double heightM = dto.Height / 100.0;
        dto.Bmi = Math.Round(dto.Weight / (heightM * heightM), 1);
        dto.BmiCategory = dto.Bmi switch
        {
            < 18.5 => "Thieu can",
            < 25.0 => "Binh thuong",
            < 30.0 => "Thua can",
            _ => "Beo phi"
        };

        // Harris-Benedict BMR
        double bmr = dto.Gender.ToLower() == "nu"
            ? 655 + (9.563 * dto.Weight) + (1.850 * dto.Height) - (4.676 * dto.Age)
            : 88.362 + (13.397 * dto.Weight) + (4.799 * dto.Height) - (5.677 * dto.Age);

        double activityMultiplier = dto.ActivityLevel switch
        {
            "LightlyActive"     => 1.375,
            "ModeratelyActive"  => 1.55,
            "VeryActive"        => 1.725,
            "ExtraActive"       => 1.9,
            _                   => 1.2   // Sedentary
        };

        double tdee = bmr * activityMultiplier;

        // Adjust by goal
        dto.Tdee = Math.Round(dto.Goal switch
        {
            "GiamCan" => tdee - 500,
            "TangCo"  => tdee + 300,
            _         => tdee           // DuyTri
        }, 0);

        return dto;
    }

    private static UserProfileDto MapToDto(UserProfile e) => new()
    {
        UserId = e.UserId,
        Age = e.Age,
        Height = e.Height,
        Weight = e.Weight,
        Gender = e.Gender,
        ActivityLevel = e.ActivityLevel,
        Goal = e.Goal
    };
}
