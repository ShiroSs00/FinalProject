using DAL.Data;
using DAL.Models;

namespace DAL.Repositories;

public class GiftRepository : IGiftRepository
{
    private readonly AppDbContext _context;

    public GiftRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Gift gift)
    {
        _context.Gifts.Add(gift);
        _context.SaveChanges();
    }

    public void Update(Gift gift)
    {
        _context.Gifts.Update(gift);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var gift = _context.Gifts.Find(id);
        if (gift != null)
        {
            _context.Gifts.Remove(gift);
            _context.SaveChanges();
        }
    }

    public Gift? GetById(int id)
    {
        return _context.Gifts.Find(id);
    }

    public List<Gift> GetAllActive()
    {
        return _context.Gifts.Where(g => g.IsActive).OrderBy(g => g.PointsRequired).ToList();
    }

    public List<Gift> GetAll()
    {
        return _context.Gifts.OrderBy(g => g.PointsRequired).ToList();
    }
}
