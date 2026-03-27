using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserGiftRepository : IUserGiftRepository
{
    private readonly AppDbContext _context;

    public UserGiftRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(UserGift userGift)
    {
        _context.UserGifts.Add(userGift);
        _context.SaveChanges();
    }

    public List<UserGift> GetByUserId(int userId)
    {
        return _context.UserGifts
            .Include(ug => ug.Gift)
            .Where(ug => ug.UserId == userId)
            .OrderByDescending(ug => ug.ExchangedAt)
            .ToList();
    }
}
