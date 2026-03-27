using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace RazorPage_test.Pages;

[Authorize]
public class MenuChatModel : PageModel
{
    private readonly INutritionService _nutritionService;
    private readonly IAiChatService _aiService;

    private const string SessionKey = "ChatHistory";

    public MenuChatModel(INutritionService nutritionService, IAiChatService aiService)
    {
        _nutritionService = nutritionService;
        _aiService = aiService;
    }

    public UserProfileDto? Profile { get; private set; }
    public List<ChatMessageDto> Messages { get; private set; } = new();
    public bool IsLoading { get; private set; }

    [BindProperty]
    public string UserMessage { get; set; } = string.Empty;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    public void OnGet()
    {
        Profile = _nutritionService.GetProfile(GetUserId());
        LoadHistory();
    }

    public async Task<IActionResult> OnPostGenMenuAsync()
    {
        if (!User.IsInRole("VIP") && !User.IsInRole("Admin"))
        {
            return RedirectToPage();
        }

        Profile = _nutritionService.GetProfile(GetUserId());
        LoadHistory();

        if (Profile == null)
            return Page();

        IsLoading = true;
        var botReply = await _aiService.GenerateDailyMenuAsync(Profile);

        AddMessage("user", "✨ Gen thuc don cho hom nay");
        AddMessage("model", botReply);
        SaveHistory();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendAsync()
    {
        if (!User.IsInRole("VIP") && !User.IsInRole("Admin"))
        {
            return RedirectToPage();
        }

        Profile = _nutritionService.GetProfile(GetUserId());
        LoadHistory();

        if (string.IsNullOrWhiteSpace(UserMessage))
            return RedirectToPage();

        AddMessage("user", UserMessage.Trim());
        SaveHistory();

        var botReply = Profile != null
            ? await _aiService.SendAsync(Profile, Messages.SkipLast(0).ToList(), UserMessage.Trim())
            : "Ban chua cap nhat ho so suc khoe. Vui long vao trang Ho so truoc nhe!";

        AddMessage("model", botReply);
        SaveHistory();

        return RedirectToPage();
    }

    public IActionResult OnPostClearChat()
    {
        HttpContext.Session.Remove(SessionKey);
        return RedirectToPage();
    }

    // ---- helpers ----
    private void LoadHistory()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        if (!string.IsNullOrEmpty(json))
            Messages = JsonSerializer.Deserialize<List<ChatMessageDto>>(json) ?? new();
    }

    private void SaveHistory()
    {
        // Keep last 40 messages to avoid huge session payloads
        if (Messages.Count > 40) Messages = Messages.TakeLast(40).ToList();
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(Messages));
    }

    private void AddMessage(string role, string content)
    {
        Messages.Add(new ChatMessageDto
        {
            Role = role,
            Content = content,
            Timestamp = DateTime.Now
        });
    }
}
