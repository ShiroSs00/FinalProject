using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required, MaxLength(50)]
    public string Type { get; set; } = string.Empty; // "VIP", "Points"

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Completed";

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
