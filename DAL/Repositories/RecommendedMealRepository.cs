using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public interface IRecommendedMealRepository
{
    List<RecommendedMeal> GetAll();
    RecommendedMeal? GetById(int id);
    RecommendedMeal? GetByType(string mealType);
    void Add(RecommendedMeal meal);
    void Update(RecommendedMeal meal);
    void Delete(int id);
}

public class RecommendedMealRepository : IRecommendedMealRepository
{
    private readonly AppDbContext _context;

    public RecommendedMealRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<RecommendedMeal> GetAll()
    {
        return _context.RecommendedMeals.ToList();
    }

    public RecommendedMeal? GetById(int id)
    {
        return _context.RecommendedMeals.Find(id);
    }

    public RecommendedMeal? GetByType(string mealType)
    {
        return _context.RecommendedMeals.FirstOrDefault(m => m.MealType == mealType);
    }

    public void Add(RecommendedMeal meal)
    {
        _context.RecommendedMeals.Add(meal);
        _context.SaveChanges();
    }

    public void Update(RecommendedMeal meal)
    {
        _context.RecommendedMeals.Update(meal);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var meal = GetById(id);
        if (meal != null)
        {
            _context.RecommendedMeals.Remove(meal);
            _context.SaveChanges();
        }
    }
}
