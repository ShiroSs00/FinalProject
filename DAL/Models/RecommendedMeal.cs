using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class RecommendedMeal
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string MealType { get; set; } = string.Empty; // "Breakfast", "Lunch", "Dinner"

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Calories { get; set; }
    
    public int Minutes { get; set; }
    
    [MaxLength(20)]
    public string Icon { get; set; } = string.Empty;
}
