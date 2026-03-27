using BLL.DTOs;
using DAL.Models;

namespace BLL.Services;

public interface IMealReviewService
{
    ReviewDto SubmitReview(int userId, string mealName, int rating, string comment);
    ReviewDto? EditReview(int id, int userId, int rating, string comment);
    bool DeleteReview(int id, int userId);
    List<ReviewDto> GetReviewsByMeal(string mealName, int currentUserId);
    List<MealReview> GetUserReviews(int userId);
    double GetAverageRating(string mealName);
}
