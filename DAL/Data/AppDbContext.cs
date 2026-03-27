using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<RecommendedMeal> RecommendedMeals { get; set; }
    public DbSet<DailyRecord> DailyRecords { get; set; }
    public DbSet<MealReview> MealReviews { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Gift> Gifts { get; set; }
    public DbSet<UserGift> UserGifts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasOne(p => p.User)
                  .WithOne()
                  .HasForeignKey<UserProfile>(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
