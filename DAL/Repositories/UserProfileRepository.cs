using DAL.Data;
using DAL.Models;

namespace DAL.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _context;

    public UserProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public UserProfile? GetByUserId(int userId)
        => _context.UserProfiles.FirstOrDefault(p => p.UserId == userId);

    public void Save(UserProfile profile)
    {
        var existing = GetByUserId(profile.UserId);
        if (existing == null)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            _context.UserProfiles.Add(profile);
        }
        else
        {
            existing.Age = profile.Age;
            existing.Height = profile.Height;
            existing.Weight = profile.Weight;
            existing.Gender = profile.Gender;
            existing.ActivityLevel = profile.ActivityLevel;
            existing.Goal = profile.Goal;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        _context.SaveChanges();
    }
}
