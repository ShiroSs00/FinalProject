using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;

    public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
    }

    public (bool Success, string Message) ProcessPayment(int userId, decimal amount, string type)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return (false, "Người dùng không tồn tại");

        if (amount <= 0) return (false, "Số tiền không hợp lệ");

        // Mock payment logic
        var transaction = new Transaction
        {
            UserId = userId,
            Amount = amount,
            Type = type,
            Status = "Completed",
            Description = type == "Points" ? $"Nạp {amount} điểm" : "Đăng ký gói VIP",
            CreatedAt = DateTime.UtcNow
        };

        _transactionRepository.Add(transaction);

        // Update user
        if (type == "Points")
        {
            user.Points += (int)amount; // 1 đồng = 1 điểm cho đơn giản
        }
        else if (type == "VIP")
        {
            user.Role = "VIP";
        }

        _userRepository.Update(user);

        return (true, "Thanh toán thành công!");
    }

    public List<Transaction> GetUserTransactions(int userId)
    {
        return _transactionRepository.GetByUserId(userId);
    }

    public List<Transaction> GetAllTransactions()
    {
        return _transactionRepository.GetAll();
    }
}
