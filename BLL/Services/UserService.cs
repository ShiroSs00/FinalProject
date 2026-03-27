using BLL.DTOs;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public (bool Success, string Error) Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            return (false, "Tên đăng nhập không được để trống.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return (false, "Email không được để trống.");

        if (_userRepository.UsernameExists(dto.Username))
            return (false, "Tên đăng nhập đã tồn tại.");

        if (_userRepository.EmailExists(dto.Email))
            return (false, "Email đã được sử dụng.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Tự động cấp quyền admin nếu username chứa "admin"
        string role = dto.Username.ToLower().Contains("admin") ? "Admin" : "User";

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            Email = dto.Email,
            FullName = dto.FullName,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _userRepository.Add(user);
        return (true, string.Empty);
    }

    public UserDto? Login(LoginDto dto)
    {
        var user = _userRepository.GetByUsername(dto.Username);
        if (user == null) return null;

        bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public DAL.Models.User? GetById(int userId)
    {
        return _userRepository.GetById(userId);
    }

    public List<DAL.Models.User> GetAllUsers()
    {
        return _userRepository.GetAll();
    }
}
