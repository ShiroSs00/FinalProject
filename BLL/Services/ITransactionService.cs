using DAL.Models;

namespace BLL.Services;

public interface ITransactionService
{
    (bool Success, string Message) ProcessPayment(int userId, decimal amount, string type);
    List<Transaction> GetUserTransactions(int userId);
    List<Transaction> GetAllTransactions();
}
