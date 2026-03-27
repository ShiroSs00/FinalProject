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
    
    // For Revenue Chart (7 days)
    public List<decimal> Last7DaysRevenue { get; set; } = new();
    public List<string> Last7DaysLabels { get; set; } = new();

    // For Role Distribution Chart
    public int CountAdmin { get; set; }
    public int CountVip { get; set; }
    public int CountUser { get; set; }

    // For Monthly Revenue Chart (6 months)
    public List<decimal> MonthlyRevenue { get; set; } = new();
    public List<string> MonthlyLabels { get; set; } = new();

    // Extra stats
    public int NewUsersThisMonth { get; set; }
    public int TotalVipPurchases { get; set; }
    public double VipConversionRate { get; set; }
    public int ActiveUsersLast7Days { get; set; }

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

        // ── Revenue Chart (7 days) ──
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var recentRevenues = _context.Transactions
            .Where(t => t.CreatedAt >= sevenDaysAgo && t.Status == "Completed")
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

        // ── Role Distribution ──
        var roles = _context.Users.GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() }).ToList();
        CountAdmin = roles.FirstOrDefault(r => r.Role == "Admin")?.Count ?? 0;
        CountVip = roles.FirstOrDefault(r => r.Role == "VIP")?.Count ?? 0;
        CountUser = roles.FirstOrDefault(r => r.Role == "User")?.Count ?? 0;

        // ── Monthly Revenue (last 6 months) ──
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);
            MonthlyLabels.Add(monthStart.ToString("MM/yyyy"));
            var sum = _context.Transactions
                .Where(t => t.CreatedAt >= monthStart && t.CreatedAt < monthEnd && t.Status == "Completed")
                .Sum(t => (decimal?)t.Amount) ?? 0;
            MonthlyRevenue.Add(sum);
        }

        // ── Extra Stats ──
        var firstOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        NewUsersThisMonth = _context.Users.Count(u => u.CreatedAt >= firstOfMonth);
        TotalVipPurchases = _context.Transactions.Count(t => t.Type == "VIP" && t.Status == "Completed");
        VipConversionRate = TotalUsers > 0 ? Math.Round((double)CountVip / TotalUsers * 100, 1) : 0;
        ActiveUsersLast7Days = _context.DailyRecords
            .Where(r => r.Date >= sevenDaysAgo)
            .Select(r => r.UserId).Distinct().Count();
    }
}
