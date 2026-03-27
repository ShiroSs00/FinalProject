using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.SignalR;
using RazorPage_test.Hubs;

namespace RazorPage_test.Services;

public interface IMenuBroadcastService
{
    Task BroadcastMenuAsync();
}

public class MenuBroadcastService : IMenuBroadcastService
{
    private readonly IRecommendedMealService _mealService;
    private readonly IHubContext<RecipeHub> _hubContext;

    public MenuBroadcastService(IRecommendedMealService mealService, IHubContext<RecipeHub> hubContext)
    {
        _mealService = mealService;
        _hubContext = hubContext;
    }

    public async Task BroadcastMenuAsync()
    {
        var all = _mealService.GetAll();
        var menuDto = new DailyMenuDto();

        var breakfast = all.FirstOrDefault(m => m.MealType == "Breakfast");
        if (breakfast != null)
        {
            menuDto.Breakfast = new RecipeDto { 
                Name = breakfast.Name, 
                Calories = breakfast.Calories, 
                Minutes = breakfast.Minutes, 
                Icon = breakfast.Icon, 
                Description = breakfast.Description 
            };
        }

        var lunch = all.FirstOrDefault(m => m.MealType == "Lunch");
        if (lunch != null)
        {
            menuDto.Lunch = new RecipeDto { 
                Name = lunch.Name, 
                Calories = lunch.Calories, 
                Minutes = lunch.Minutes, 
                Icon = lunch.Icon, 
                Description = lunch.Description 
            };
        }

        var dinner = all.FirstOrDefault(m => m.MealType == "Dinner");
        if (dinner != null)
        {
            menuDto.Dinner = new RecipeDto { 
                Name = dinner.Name, 
                Calories = dinner.Calories, 
                Minutes = dinner.Minutes, 
                Icon = dinner.Icon, 
                Description = dinner.Description 
            };
        }

        await _hubContext.Clients.All.SendAsync("ReceiveNewMenu", menuDto);
    }
}
