using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;

        public SaleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sale?> GetSaleByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.InvoiceNumber == invoiceNumber);
        }

        public async Task<Sale> CreateSaleAsync(Sale sale, List<SaleDetail> saleDetails)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Generate invoice number
                sale.InvoiceNumber = await GenerateInvoiceNumberAsync();
                sale.SaleDate = DateTime.Now;
                sale.Status = true;

                // Calculate totals
                sale.Subtotal = saleDetails.Sum(sd => sd.Total);
                sale.GrandTotal = sale.Subtotal - sale.Discount + sale.Tax;
                sale.ChangeAmount = sale.PaidAmount - sale.GrandTotal;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Add sale details and update stock
                foreach (var detail in saleDetails)
                {
                    detail.SaleId = sale.Id;
                    detail.Total = detail.Quantity * detail.UnitPrice - detail.Discount;
                    _context.SaleDetails.Add(detail);

                    // Update product stock
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.StockQty -= detail.Quantity;
                        
                        // Add inventory log
                        var log = new InventoryLog
                        {
                            ProductId = product.Id,
                            UserId = sale.UserId,
                            TransactionType = "Sale",
                            Quantity = detail.Quantity,
                            PreviousStock = product.StockQty + detail.Quantity,
                            NewStock = product.StockQty,
                            Notes = $"Sale #{sale.InvoiceNumber}"
                        };
                        _context.InventoryLogs.Add(log);
                    }
                }

                // Update customer loyalty points if applicable
                if (sale.CustomerId.HasValue)
                {
                    var customer = await _context.Customers.FindAsync(sale.CustomerId);
                    if (customer != null)
                    {
                        customer.LoyaltyPoints += (int)(sale.GrandTotal / 100); // 1 point per 100 rupees
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return sale;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status && s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByProductAsync(int productId)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status && s.SaleDetails.Any(sd => sd.ProductId == productId))
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTodaySalesAsync()
        {
            var today = DateTime.Today;
            return await _context.Sales
                .Where(s => s.Status && s.SaleDate.Date == today)
                .SumAsync(s => s.GrandTotal);
        }

        public async Task<decimal> GetTodayProfitAsync()
        {
            var today = DateTime.Today;
            var sales = await _context.Sales
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status && s.SaleDate.Date == today)
                .ToListAsync();

            decimal profit = 0;
            foreach (var sale in sales)
            {
                foreach (var detail in sale.SaleDetails)
                {
                    if (detail.Product != null)
                    {
                        profit += (detail.UnitPrice - detail.Product.PurchasePrice) * detail.Quantity;
                    }
                }
            }

            return profit;
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            return await _context.Sales
                .Where(s => s.Status && s.SaleDate.Month == month && s.SaleDate.Year == year)
                .SumAsync(s => s.GrandTotal);
        }

        public async Task<IEnumerable<Sale>> GetRecentTransactionsAsync(int count)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                .Where(s => s.Status)
                .OrderByDescending(s => s.SaleDate)
                .Take(count)
                .ToListAsync();
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var today = DateTime.Today;
            var count = await _context.Sales
                .CountAsync(s => s.SaleDate.Date == today);
            
            return $"INV-{today:yyyyMMdd}-{(count + 1):D4}";
        }
    }
}
