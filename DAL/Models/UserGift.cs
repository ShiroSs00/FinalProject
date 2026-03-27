using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class UserGift
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    public int GiftId { get; set; }

    [ForeignKey(nameof(GiftId))]
    public Gift? Gift { get; set; }

    public DateTime ExchangedAt { get; set; } = DateTime.UtcNow;
}
