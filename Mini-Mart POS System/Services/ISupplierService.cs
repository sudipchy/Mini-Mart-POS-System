using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);
        Task<IEnumerable<Purchase>> GetSupplierPurchaseHistoryAsync(int supplierId);
    }
}
