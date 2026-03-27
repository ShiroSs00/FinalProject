using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public class GiftService : IGiftService
{
    private readonly IGiftRepository _giftRepository;
    private readonly IUserGiftRepository _userGiftRepository;
    private readonly IUserRepository _userRepository;

    public GiftService(IGiftRepository giftRepository, IUserGiftRepository userGiftRepository, IUserRepository userRepository)
    {
        _giftRepository = giftRepository;
        _userGiftRepository = userGiftRepository;
        _userRepository = userRepository;
    }

    public List<Gift> GetAllActiveGifts() => _giftRepository.GetAllActive();
    
    public List<Gift> GetAllGifts() => _giftRepository.GetAll();

    public Gift? GetGiftById(int id) => _giftRepository.GetById(id);

    public void AddGift(Gift gift) => _giftRepository.Add(gift);

    public void UpdateGift(Gift gift) => _giftRepository.Update(gift);

    public void DeleteGift(int id) => _giftRepository.Delete(id);

    public (bool Success, string Message) ExchangeGift(int userId, int giftId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return (false, "Người dùng không tồn tại.");

        var gift = _giftRepository.GetById(giftId);
        if (gift == null || !gift.IsActive) return (false, "Quà tặng không tồn tại hoặc đã ngừng đổi.");

        if (gift.Stock == 0) return (false, "Quà tặng đã hết hàng.");

        if (user.Points < gift.PointsRequired) return (false, "Không đủ điểm để đổi món quà này.");

        // Deduct points
        user.Points -= gift.PointsRequired;
        _userRepository.Update(user);

        // Deduct stock if limited
        if (gift.Stock > 0)
        {
            gift.Stock -= 1;
            _giftRepository.Update(gift);
        }

        // Add history
        var userGift = new UserGift
        {
            UserId = userId,
            GiftId = giftId,
            ExchangedAt = DateTime.UtcNow
        };
        _userGiftRepository.Add(userGift);

        return (true, "Đổi quà thành công!");
    }

    public List<UserGift> GetUserGiftHistory(int userId)
    {
        return _userGiftRepository.GetByUserId(userId);
    }
}
