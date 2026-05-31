using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductByBarcodeAsync(string barcode);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task ImportProductsFromExcelAsync(string filePath);
        Task<byte[]> ExportProductsToExcelAsync();
    }
}
