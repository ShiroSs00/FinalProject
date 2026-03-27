using BLL.DTOs;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public class MealReviewService : IMealReviewService
{
    private readonly IMealReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public MealReviewService(IMealReviewRepository reviewRepository, IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
    }

    public ReviewDto SubmitReview(int userId, string mealName, int rating, string comment)
    {
        var review = new MealReview
        {
            UserId = userId,
            MealName = mealName,
            Rating = rating,
            Comment = comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _reviewRepository.Add(review);

        // Gamification: +10 điểm cho mỗi lần đánh giá
        var user = _userRepository.GetById(userId);
        if (user != null)
        {
            user.Points += 10;
            _userRepository.Update(user);
        }

        return new ReviewDto
        {
            Id = review.Id,
            UserId = userId,
            UserName = user?.FullName ?? "Unknown",
            UserInitial = string.IsNullOrEmpty(user?.FullName) ? "U" : user.FullName[0].ToString().ToUpper(),
            MealName = mealName,
            Rating = rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            IsMine = true
        };
    }

    public ReviewDto? EditReview(int id, int userId, int rating, string comment)
    {
        var review = _reviewRepository.GetById(id);
        if (review == null || review.UserId != userId) return null;

        review.Rating = rating;
        review.Comment = comment ?? string.Empty;
        _reviewRepository.Update(review);

        var user = _userRepository.GetById(userId);

        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = user?.FullName ?? "Unknown",
            UserInitial = string.IsNullOrEmpty(user?.FullName) ? "U" : user.FullName[0].ToString().ToUpper(),
            MealName = review.MealName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            IsMine = true
        };
    }

    public bool DeleteReview(int id, int userId)
    {
        var review = _reviewRepository.GetById(id);
        if (review == null || review.UserId != userId) return false;

        _reviewRepository.Delete(id);
        return true;
    }

    public List<ReviewDto> GetReviewsByMeal(string mealName, int currentUserId)
    {
        var reviews = _reviewRepository.GetByMealName(mealName);
        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User != null ? r.User.FullName : "Unknown",
            UserInitial = r.User != null && !string.IsNullOrEmpty(r.User.FullName) ? r.User.FullName[0].ToString().ToUpper() : "U",
            MealName = r.MealName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            IsMine = r.UserId == currentUserId
        }).ToList();
    }

    public List<MealReview> GetUserReviews(int userId)
    {
        return _reviewRepository.GetByUserId(userId);
    }

    public double GetAverageRating(string mealName)
    {
        return _reviewRepository.GetAverageRating(mealName);
    }
}
