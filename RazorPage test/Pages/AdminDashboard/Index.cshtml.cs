using BLL.Services;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPage_test.Pages.AdminDashboard;

[Authorize]
public class DashboardOverviewModel : PageModel
{
    private readonly AppDbContext _context;

    public DashboardOverviewModel(AppDbContext context)
    {
        _context = context;
    }

    public int TotalUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalExchanged { get; set; }
    public int TotalReviews { get; set; }
    
    public List<Transaction> RecentTransactions { get; set; } = new();
    
    // For Chart
    public List<decimal> Last7DaysRevenue { get; set; } = new();
    public List<string> Last7DaysLabels { get; set; } = new();

    public void OnGet()
    {
        // Simple dashboard queries using DbContext directly for speed in admin overview
        TotalUsers = _context.Users.Count();
        TotalRevenue = _context.Transactions.Sum(t => t.Amount);
        TotalExchanged = _context.UserGifts.Count();
        TotalReviews = _context.MealReviews.Count();

        RecentTransactions = _context.Transactions
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToList();

        // Chart Data Calculation
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var recentRevenues = _context.Transactions
            .Where(t => t.CreatedAt >= sevenDaysAgo && t.Status == "Success")
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(t => t.Amount) })
            .ToList();
            
        for (int i = 6; i >= 0; i--)
        {
            var dat = DateTime.UtcNow.AddDays(-i).Date;
            Last7DaysLabels.Add(dat.ToString("dd/MM"));
            var match = recentRevenues.FirstOrDefault(r => r.Date == dat);
            Last7DaysRevenue.Add(match != null ? match.Total : 0);
        }
    }
}
