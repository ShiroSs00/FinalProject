using DAL.Data;
using DAL.Models;

namespace DAL.Repositories;

public interface IDailyRecordRepository
{
    DailyRecord? GetByDate(int userId, DateTime date);
    List<DailyRecord> GetRecentRecords(int userId, int days);
    void Add(DailyRecord record);
    void Update(DailyRecord record);
}

public class DailyRecordRepository : IDailyRecordRepository
{
    private readonly AppDbContext _context;

    public DailyRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public DailyRecord? GetByDate(int userId, DateTime date)
    {
        return _context.DailyRecords
            .FirstOrDefault(r => r.UserId == userId && r.Date.Date == date.Date);
    }

    public List<DailyRecord> GetRecentRecords(int userId, int days)
    {
        var cutoff = DateTime.Now.Date.AddDays(-days + 1);
        return _context.DailyRecords
            .Where(r => r.UserId == userId && r.Date.Date >= cutoff)
            .OrderBy(r => r.Date)
            .ToList();
    }

    public void Add(DailyRecord record)
    {
        _context.DailyRecords.Add(record);
        _context.SaveChanges();
    }

    public void Update(DailyRecord record)
    {
        _context.DailyRecords.Update(record);
        _context.SaveChanges();
    }
}
