using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetAllSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<Sale?> GetSaleByInvoiceNumberAsync(string invoiceNumber);
        Task<Sale> CreateSaleAsync(Sale sale, List<SaleDetail> saleDetails);
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sale>> GetSalesByProductAsync(int productId);
        Task<decimal> GetTodaySalesAsync();
        Task<decimal> GetTodayProfitAsync();
        Task<decimal> GetMonthlyRevenueAsync(int month, int year);
        Task<IEnumerable<Sale>> GetRecentTransactionsAsync(int count);
    }
}
