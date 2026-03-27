using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class Gift
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int PointsRequired { get; set; }

    [MaxLength(200)]
    public string Icon { get; set; } = string.Empty;

    public int Stock { get; set; } = -1; // -1 means unlimited

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
