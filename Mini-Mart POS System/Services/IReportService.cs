using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateDailySalesReportAsync(DateTime date);
        Task<byte[]> GenerateMonthlySalesReportAsync(int month, int year);
        Task<byte[]> GenerateStockReportAsync();
        Task<byte[]> GenerateTopSellingProductsReportAsync(int topCount = 10);
        Task<byte[]> GenerateProfitReportAsync(DateTime startDate, DateTime endDate);
    }
}
