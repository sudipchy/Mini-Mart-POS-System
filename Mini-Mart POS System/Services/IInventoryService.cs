using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryLog>> GetInventoryLogsAsync();
        Task<IEnumerable<InventoryLog>> GetInventoryLogsByProductAsync(int productId);
        Task StockInAsync(int productId, int quantity, string userId, string? notes = null);
        Task StockOutAsync(int productId, int quantity, string userId, string? notes = null);
        Task AdjustStockAsync(int productId, int newQuantity, string userId, string? notes = null);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync();
        Task<IEnumerable<Product>> GetExpiringProductsAsync(int daysBeforeExpiry = 30);
    }
}
