namespace BLL.DTOs;

public class UserProfileDto
{
    public int UserId { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public string Gender { get; set; } = "Nam";
    public string ActivityLevel { get; set; } = "Sedentary";
    public string Goal { get; set; } = "DuyTri";

    // Calculated
    public double Bmi { get; set; }
    public double Tdee { get; set; }
    public string BmiCategory { get; set; } = string.Empty;
}

public class ChatMessageDto
{
    public string Role { get; set; } = "user"; // "user" | "model"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
