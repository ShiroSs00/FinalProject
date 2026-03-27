using DAL.Models;

namespace DAL.Repositories;

public interface IUserGiftRepository
{
    void Add(UserGift userGift);
    List<UserGift> GetByUserId(int userId);
}
