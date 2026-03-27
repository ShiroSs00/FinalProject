namespace BLL.DTOs;

public class RecipeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Calories { get; set; }
    public int Minutes { get; set; }
    public string Icon { get; set; } = "🍲"; // Emoji or image URL
}

public class DailyMenuDto
{
    public RecipeDto Breakfast { get; set; } = new();
    public RecipeDto Lunch { get; set; } = new();
    public RecipeDto Dinner { get; set; } = new();
}
