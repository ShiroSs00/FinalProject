using DAL.Data;
using DAL.Models;

namespace DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
        => _context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

    public User? GetByEmail(string email)
        => _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

    public User? GetById(int userId)
        => _context.Users.FirstOrDefault(u => u.Id == userId);

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public bool UsernameExists(string username)
        => _context.Users.Any(u => u.Username.ToLower() == username.ToLower());

    public bool EmailExists(string email)
        => _context.Users.Any(u => u.Email.ToLower() == email.ToLower());

    public List<User> GetAll()
    {
        return _context.Users.OrderByDescending(u => u.CreatedAt).ToList();
    }
}
