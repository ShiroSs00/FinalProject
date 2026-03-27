using DAL.Models;

namespace BLL.Services;

public interface IGiftService
{
    List<Gift> GetAllActiveGifts();
    List<Gift> GetAllGifts();
    (bool Success, string Message) ExchangeGift(int userId, int giftId);
    List<UserGift> GetUserGiftHistory(int userId);
    void AddGift(Gift gift);
    void UpdateGift(Gift gift);
    void DeleteGift(int id);
    Gift? GetGiftById(int id);
}
