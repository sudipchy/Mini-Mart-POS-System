using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.Status)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            supplier.CreatedDate = DateTime.Now;
            supplier.Status = true;
            
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            
            return supplier;
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(supplier.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Supplier not found");
            }

            existing.SupplierName = supplier.SupplierName;
            existing.Phone = supplier.Phone;
            existing.Email = supplier.Email;
            existing.Address = supplier.Address;
            existing.ContactPerson = supplier.ContactPerson;
            existing.Status = supplier.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                supplier.Status = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Purchase>> GetSupplierPurchaseHistoryAsync(int supplierId)
        {
            return await _context.Purchases
                .Include(p => p.User)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .Where(p => p.SupplierId == supplierId && p.Status)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
        }
    }
}
