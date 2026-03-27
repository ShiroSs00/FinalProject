using DAL.Models;

namespace DAL.Repositories;

public interface IUserRepository
{
    User? GetByUsername(string username);
    User? GetByEmail(string email);
    User? GetById(int userId);
    void Add(User user);
    void Update(User user);
    bool UsernameExists(string username);
    bool EmailExists(string email);
    List<User> GetAll();
}
