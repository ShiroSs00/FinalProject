using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class MealReviewRepository : IMealReviewRepository
{
    private readonly AppDbContext _context;

    public MealReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(MealReview review)
    {
        _context.MealReviews.Add(review);
        _context.SaveChanges();
    }

    public void Update(MealReview review)
    {
        _context.MealReviews.Update(review);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var review = _context.MealReviews.Find(id);
        if (review != null)
        {
            _context.MealReviews.Remove(review);
            _context.SaveChanges();
        }
    }

    public MealReview? GetById(int id)
    {
        return _context.MealReviews.Find(id);
    }

    public List<MealReview> GetByUserId(int userId)
    {
        return _context.MealReviews.Where(r => r.UserId == userId).ToList();
    }

    public List<MealReview> GetByMealName(string mealName)
    {
        return _context.MealReviews
            .Include(r => r.User)
            .Where(r => r.MealName == mealName)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public double GetAverageRating(string mealName)
    {
        var reviews = _context.MealReviews.Where(r => r.MealName == mealName);
        if (!reviews.Any()) return 0;
        return reviews.Average(r => r.Rating);
    }
}
