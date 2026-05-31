using MiniMartPOS.Models;

namespace MiniMartPOS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public decimal TodayProfit { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockAlerts { get; set; }
        public List<Sale> RecentTransactions { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
        public decimal MonthlyRevenue { get; set; }
        public bool BackupStatus { get; set; } = true;
    }
}
