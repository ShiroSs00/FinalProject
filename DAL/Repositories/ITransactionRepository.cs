using DAL.Models;

namespace DAL.Repositories;

public interface ITransactionRepository
{
    void Add(Transaction transaction);
    List<Transaction> GetByUserId(int userId);
    List<Transaction> GetAll();
}
