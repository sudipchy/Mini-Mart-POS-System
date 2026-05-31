using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryLog>> GetInventoryLogsAsync()
        {
            return await _context.InventoryLogs
                .Include(il => il.Product)
                .Include(il => il.User)
                .OrderByDescending(il => il.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryLog>> GetInventoryLogsByProductAsync(int productId)
        {
            return await _context.InventoryLogs
                .Include(il => il.Product)
                .Include(il => il.User)
                .Where(il => il.ProductId == productId)
                .OrderByDescending(il => il.TransactionDate)
                .ToListAsync();
        }

        public async Task StockInAsync(int productId, int quantity, string userId, string? notes = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }

            var previousStock = product.StockQty;
            product.StockQty += quantity;

            var log = new InventoryLog
            {
                ProductId = productId,
                UserId = userId,
                TransactionType = "Stock In",
                Quantity = quantity,
                PreviousStock = previousStock,
                NewStock = product.StockQty,
                Notes = notes ?? "Stock received"
            };

            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task StockOutAsync(int productId, int quantity, string userId, string? notes = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }

            if (product.StockQty < quantity)
            {
                throw new InvalidOperationException("Insufficient stock");
            }

            var previousStock = product.StockQty;
            product.StockQty -= quantity;

            var log = new InventoryLog
            {
                ProductId = productId,
                UserId = userId,
                TransactionType = "Stock Out",
                Quantity = quantity,
                PreviousStock = previousStock,
                NewStock = product.StockQty,
                Notes = notes ?? "Stock removed"
            };

            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AdjustStockAsync(int productId, int newQuantity, string userId, string? notes = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }

            var previousStock = product.StockQty;
            product.StockQty = newQuantity;

            var log = new InventoryLog
            {
                ProductId = productId,
                UserId = userId,
                TransactionType = "Stock Adjustment",
                Quantity = newQuantity - previousStock,
                PreviousStock = previousStock,
                NewStock = newQuantity,
                Notes = notes ?? "Stock adjusted"
            };

            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status && p.StockQty == 0)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetExpiringProductsAsync(int daysBeforeExpiry = 30)
        {
            var expiryDate = DateTime.Today.AddDays(daysBeforeExpiry);
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status && p.ExpiryDate.HasValue && p.ExpiryDate <= expiryDate)
                .OrderBy(p => p.ExpiryDate)
                .ToListAsync();
        }
    }
}
