using DAL.Models;

namespace DAL.Repositories;

public interface IGiftRepository
{
    void Add(Gift gift);
    void Update(Gift gift);
    void Delete(int id);
    Gift? GetById(int id);
    List<Gift> GetAllActive();
    List<Gift> GetAll();
}
