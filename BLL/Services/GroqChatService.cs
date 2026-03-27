using BLL.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BLL.Services;

public interface IAiChatService
{
    Task<string> SendAsync(UserProfileDto profile, List<ChatMessageDto> history, string userMessage);
    Task<string> GenerateDailyMenuAsync(UserProfileDto profile);
}

public class GroqChatService : IAiChatService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly DAL.Repositories.IMealReviewRepository _reviewRepository;
    private const string ModelName = "llama-3.3-70b-versatile"; 
    private const string BaseUrl = "https://api.groq.com/openai/v1/chat/completions";

    public GroqChatService(IHttpClientFactory httpFactory, IConfiguration config, DAL.Repositories.IMealReviewRepository reviewRepository)
    {
        _http = httpFactory.CreateClient("Groq");
        _apiKey = config["GroqApiKey"] ?? string.Empty;
        _reviewRepository = reviewRepository;
    }

    public async Task<string> GenerateDailyMenuAsync(UserProfileDto profile)
    {
        var goalText = profile.Goal switch
        {
            "GiamCan" => "giam cân",
            "TangCo"  => "tăng cơ bắp",
            _         => "duy trì cân nặng"
        };
        var actText = profile.ActivityLevel switch
        {
            "LightlyActive"    => "vận động nhẹ (1-3 buổi/tuần)",
            "ModeratelyActive" => "vận động vừa (3-5 buổi/tuần)",
            "VeryActive"       => "vận động nhiều (6-7 buổi/tuần)",
            "ExtraActive"      => "vận động rất nhiều (2 ca/ngày)",
            _                  => "ít vận động (việc văn phòng)"
        };

        var systemPrompt = $@"Bạn là chuyên gia dinh dưỡng hàng đầu Việt Nam. 
Hãy tạo thực đơn 1 ngày đầy đủ (bữa sáng, bữa trưa, bữa tối) 
phù hợp với chỉ số sau của người dùng:
- Giới tính: {profile.Gender}, Tuổi: {profile.Age}
- Chỉ số: {profile.Weight}kg, {profile.Height}cm (BMI: {profile.Bmi})
- Mục tiêu: {goalText}, TDEE: {profile.Tdee} kcal
";

        // Thêm thông tin từ review cũ để AI học hỏi
        var recentReviews = _reviewRepository.GetByUserId(profile.UserId).TakeLast(5).ToList();
        if (recentReviews.Any())
        {
            var reviewContext = string.Join("\n", recentReviews.Select(r => $"- Món '{r.MealName}': {r.Rating}/5 sao. Nhận xét: {r.Comment}"));
            systemPrompt += $"\nLưu ý lịch sử đánh giá của người dùng để điều chỉnh (Ưu tiên món điểm cao, tránh món điểm thấp):\n{reviewContext}\n";
        }

        systemPrompt += @"
Yêu cầu cực kì quan trọng:
1. Dùng món ăn Việt Nam thông thường, dễ mua nguyên liệu
2. Ghi rõ tên món, nguyên liệu, lượng ước tính và calo của từng món
3. Tổng calo PHẢI GẦN BẰNG TDEE ({profile.Tdee} kcal)
4. Trình bày đẹp bằng Markdown
5. LUÔN TRẢ LỜI BẰNG TIẾNG VIỆT.";

        return await CallGroqAsync(systemPrompt, new List<ChatMessageDto>(),
            "Hãy tạo thực đơn cho tôi trong ngày hôm nay.");
    }

    public async Task<string> SendAsync(UserProfileDto profile, List<ChatMessageDto> history, string userMessage)
    {
        var systemPrompt = $@"Bạn là tư vấn viên dinh dưỡng người Việt. 
Khách hàng có: {profile.Gender}, {profile.Age} tuổi, Cao {profile.Height}cm, Nặng {profile.Weight}kg, 
BMI {profile.Bmi} ({profile.BmiCategory}), TDEE {profile.Tdee} kcal/ngày, Mục tiêu: {profile.Goal}.
Trả lời ngắn gọn, thân thiện, mang tính thực tế. LUÔN TRẢ LỜI BẰNG TIẾNG VIỆT.";

        return await CallGroqAsync(systemPrompt, history, userMessage);
    }

    private async Task<string> CallGroqAsync(string systemInstruction, List<ChatMessageDto> history, string userMessage)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return "⚠️ Chưa cấu hình Groq API Key. Vui lòng thêm GroqApiKey vào appsettings.json.";

        var messages = new List<object>
        {
            new { role = "system", content = systemInstruction }
        };

        foreach (var msg in history)
        {
            // Gemini dùng 'model', OpenAI/Groq dùng 'assistant'
            var role = msg.Role == "model" ? "assistant" : "user";
            messages.Add(new { role = role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = userMessage });

        var body = new
        {
            model = ModelName,
            messages = messages,
            temperature = 0.7,
            max_tokens = 2048
        };

        var jsonBody = JsonSerializer.Serialize(body);
        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            var response = await _http.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Lỗi API ({response.StatusCode}): {result}";

            using var doc = JsonDocument.Parse(result);
            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text ?? "Không nhận được phản hồi.";
        }
        catch (Exception ex)
        {
            return $"Lỗi kết nối: {ex.Message}";
        }
    }
}
