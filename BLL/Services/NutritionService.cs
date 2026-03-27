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
        bool isFemale = dto.Gender.ToLower() == "nu";
        double bmr = isFemale
            ? 655 + (9.563 * dto.Weight) + (1.850 * dto.Height) - (4.676 * dto.Age)
            : 88.362 + (13.397 * dto.Weight) + (4.799 * dto.Height) - (5.677 * dto.Age);
        dto.Bmr = Math.Round(bmr, 0);

        double activityMultiplier = dto.ActivityLevel switch
        {
            "LightlyActive"     => 1.375,
            "ModeratelyActive"  => 1.55,
            "VeryActive"        => 1.725,
            "ExtraActive"       => 1.9,
            _                   => 1.2   // Sedentary
        };

        double tdee = bmr * activityMultiplier;
        dto.TdeeBeforeGoal = Math.Round(tdee, 0);

        // Adjust by goal
        double adjustedTdee = dto.Goal switch
        {
            "GiamCan" => tdee - 500,
            "TangCo"  => tdee + 300,
            _         => tdee           // DuyTri
        };
        dto.Tdee = Math.Round(adjustedTdee, 0);

        // Ideal Body Weight (Devine formula)
        double heightInches = dto.Height / 2.54;
        dto.IdealWeight = isFemale
            ? Math.Round(45.5 + 2.3 * (heightInches - 60), 1)
            : Math.Round(50.0 + 2.3 * (heightInches - 60), 1);
        if (dto.IdealWeight < 0) dto.IdealWeight = 0;

        // Body Fat % estimate (BMI-based: Deurenberg formula)
        dto.BodyFatPercentage = Math.Round(
            (1.20 * dto.Bmi) + (0.23 * dto.Age) - (isFemale ? 5.4 : 16.2), 1);
        if (dto.BodyFatPercentage < 3) dto.BodyFatPercentage = 3;

        // Daily water intake (approx 0.033 L per kg body weight)
        dto.DailyWaterLiters = Math.Round(dto.Weight * 0.033, 1);

        // Macro breakdown based on adjusted TDEE
        // Protein: 25%, Carbs: 50%, Fat: 25%
        dto.ProteinGrams = Math.Round(adjustedTdee * 0.25 / 4, 0);   // 4 kcal/g
        dto.CarbGrams    = Math.Round(adjustedTdee * 0.50 / 4, 0);   // 4 kcal/g
        dto.FatGrams     = Math.Round(adjustedTdee * 0.25 / 9, 0);   // 9 kcal/g

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
