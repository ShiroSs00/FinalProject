using DAL.Models;

namespace DAL.Repositories;

public interface IUserProfileRepository
{
    UserProfile? GetByUserId(int userId);
    void Save(UserProfile profile); // Add or Update
}
