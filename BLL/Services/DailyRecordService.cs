using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public interface IDailyRecordService
{
    DailyRecord GetTodayRecord(int userId);
    void LogCalories(int userId, int caloriesToAdd);
    List<DailyRecord> GetLast7Days(int userId);
}

public class DailyRecordService : IDailyRecordService
{
    private readonly IDailyRecordRepository _repository;

    public DailyRecordService(IDailyRecordRepository repository)
    {
        _repository = repository;
    }

    public DailyRecord GetTodayRecord(int userId)
    {
        var record = _repository.GetByDate(userId, DateTime.Now.Date);
        if (record == null)
        {
            record = new DailyRecord
            {
                UserId = userId,
                Date = DateTime.Now.Date,
                CaloriesConsumed = 0
            };
            _repository.Add(record);
        }
        return record;
    }

    public void LogCalories(int userId, int caloriesToAdd)
    {
        var record = GetTodayRecord(userId);
        record.CaloriesConsumed += caloriesToAdd;
        _repository.Update(record);
    }

    public List<DailyRecord> GetLast7Days(int userId)
    {
        return _repository.GetRecentRecords(userId, 7);
    }
}
