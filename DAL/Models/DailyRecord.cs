using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class DailyRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    public DateTime Date { get; set; } // Ngày ghi chú (chỉ lấy phần Date)

    [Required]
    public int CaloriesConsumed { get; set; } // Tổng calo nạp vào ngày hôm đó
}
