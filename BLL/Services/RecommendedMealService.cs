using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public interface IRecommendedMealService
{
    List<RecommendedMeal> GetAll();
    RecommendedMeal? GetById(int id);
    RecommendedMeal? GetByType(string mealType);
    bool Create(RecommendedMeal meal);
    void Update(RecommendedMeal meal);
    void Delete(int id);
    List<string> GetAvailableMealTypes();
}

public class RecommendedMealService : IRecommendedMealService
{
    private readonly IRecommendedMealRepository _repository;

    public RecommendedMealService(IRecommendedMealRepository repository)
    {
        _repository = repository;
    }

    public List<RecommendedMeal> GetAll() => _repository.GetAll();

    public RecommendedMeal? GetById(int id) => _repository.GetById(id);

    public RecommendedMeal? GetByType(string mealType) => _repository.GetByType(mealType);

    public bool Create(RecommendedMeal meal)
    {
        // Kiểm tra xem MealType này đã tồn tại chưa
        if (_repository.GetByType(meal.MealType) != null)
        {
            return false; // Đã tồn tại, không cho tạo
        }
        
        _repository.Add(meal);
        return true;
    }

    public void Update(RecommendedMeal meal) => _repository.Update(meal);

    public void Delete(int id) => _repository.Delete(id);

    public List<string> GetAvailableMealTypes()
    {
        var allTypes = new List<string> { "Breakfast", "Lunch", "Dinner" };
        var existing = _repository.GetAll().Select(m => m.MealType).ToList();
        return allTypes.Except(existing).ToList();
    }
}
