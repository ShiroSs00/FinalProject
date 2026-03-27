using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class UserProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public int Age { get; set; }

    /// <summary>Chieu cao (cm)</summary>
    public double Height { get; set; }

    /// <summary>Can nang (kg)</summary>
    public double Weight { get; set; }

    /// <summary>Nam / Nu</summary>
    [MaxLength(10)]
    public string Gender { get; set; } = "Nam";

    /// <summary>
    /// Muc do van dong:
    /// Sedentary | LightlyActive | ModeratelyActive | VeryActive | ExtraActive
    /// </summary>
    [MaxLength(30)]
    public string ActivityLevel { get; set; } = "Sedentary";

    /// <summary>
    /// Muc tieu: GiamCan | DuyTri | TangCo
    /// </summary>
    [MaxLength(20)]
    public string Goal { get; set; } = "DuyTri";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
