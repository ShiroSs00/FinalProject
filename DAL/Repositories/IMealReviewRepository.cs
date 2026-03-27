using DAL.Models;

namespace DAL.Repositories;

public interface IMealReviewRepository
{
    void Add(MealReview review);
    void Update(MealReview review);
    void Delete(int id);
    MealReview? GetById(int id);
    List<MealReview> GetByUserId(int userId);
    List<MealReview> GetByMealName(string mealName);
    double GetAverageRating(string mealName);
}
