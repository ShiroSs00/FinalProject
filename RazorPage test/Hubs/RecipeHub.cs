using BLL.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace RazorPage_test.Hubs;

public class RecipeHub : Hub
{
    // Admin có thể gọi hàm này trực tiếp từ code-behind bằng IHubContext
    // Không cần định nghĩa method ở đây trừ khi Client muốn gửi tin lên.
}
