using BLL.DTOs;

namespace BLL.Services;

public interface IUserService
{
    /// <summary>
    /// Đăng ký người dùng mới. Trả về (true, "") nếu thành công, (false, lý do) nếu thất bại.
    /// </summary>
    (bool Success, string Error) Register(RegisterDto dto);

    UserDto? Login(LoginDto dto);

    DAL.Models.User? GetById(int userId);
    List<DAL.Models.User> GetAllUsers();
}
