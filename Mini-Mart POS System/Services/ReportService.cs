using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateDailySalesReportAsync(DateTime date)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            document.Add(new Paragraph($"Daily Sales Report - {date:yyyy-MM-dd}", titleFont));
            document.Add(Chunk.NEWLINE);

            // Get sales data
            var sales = await _context.Sales
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.User)
                .Where(s => s.Status && s.SaleDate.Date == date)
                .OrderBy(s => s.SaleDate)
                .ToListAsync();

            // Create table
            var table = new PdfPTable(6);
            table.WidthPercentage = 100;

            table.AddCell("Invoice #");
            table.AddCell("Time");
            table.AddCell("Items");
            table.AddCell("Total");
            table.AddCell("Payment");
            table.AddCell("Cashier");

            decimal totalSales = 0;
            foreach (var sale in sales)
            {
                table.AddCell(sale.InvoiceNumber);
                table.AddCell(sale.SaleDate.ToString("HH:mm"));
                table.AddCell(sale.SaleDetails.Count.ToString());
                table.AddCell($"Rs. {sale.GrandTotal:N2}");
                table.AddCell(sale.PaymentMethod);
                table.AddCell(sale.User?.FullName ?? "N/A");
                totalSales += sale.GrandTotal;
            }

            document.Add(table);
            document.Add(Chunk.NEWLINE);

            // Summary
            document.Add(new Paragraph($"Total Sales: Rs. {totalSales:N2}", titleFont));
            document.Add(new Paragraph($"Total Transactions: {sales.Count}", normalFont));

            document.Close();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateMonthlySalesReportAsync(int month, int year)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            document.Add(new Paragraph($"Monthly Sales Report - {new DateTime(year, month, 1):MMMM yyyy}", titleFont));
            document.Add(Chunk.NEWLINE);

            var sales = await _context.Sales
                .Include(s => s.SaleDetails)
                .Where(s => s.Status && s.SaleDate.Month == month && s.SaleDate.Year == year)
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(s => s.GrandTotal), Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            var table = new PdfPTable(3);
            table.WidthPercentage = 100;

            table.AddCell("Date");
            table.AddCell("Sales");
            table.AddCell("Transactions");

            decimal totalRevenue = 0;
            int totalTransactions = 0;

            foreach (var day in sales)
            {
                table.AddCell(day.Date.ToString("yyyy-MM-dd"));
                table.AddCell($"Rs. {day.Total:N2}");
                table.AddCell(day.Count.ToString());
                totalRevenue += day.Total;
                totalTransactions += day.Count;
            }

            document.Add(table);
            document.Add(Chunk.NEWLINE);

            document.Add(new Paragraph($"Total Revenue: Rs. {totalRevenue:N2}", titleFont));
            document.Add(new Paragraph($"Total Transactions: {totalTransactions}", normalFont));

            document.Close();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateStockReportAsync()
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            document.Add(new Paragraph("Stock Report", titleFont));
            document.Add(Chunk.NEWLINE);

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status)
                .OrderBy(p => p.Category.CategoryName)
                .ThenBy(p => p.ProductName)
                .ToListAsync();

            var table = new PdfPTable(7);
            table.WidthPercentage = 100;

            table.AddCell("Barcode");
            table.AddCell("Product");
            table.AddCell("Category");
            table.AddCell("Stock");
            table.AddCell("Min Stock");
            table.AddCell("Price");
            table.AddCell("Status");

            foreach (var product in products)
            {
                table.AddCell(product.Barcode ?? "N/A");
                table.AddCell(product.ProductName);
                table.AddCell(product.Category?.CategoryName ?? "N/A");
                table.AddCell(product.StockQty.ToString());
                table.AddCell(product.MinimumStock.ToString());
                table.AddCell($"Rs. {product.SellingPrice:N2}");
                
                var status = product.StockQty == 0 ? "Out of Stock" :
                            product.StockQty <= product.MinimumStock ? "Low Stock" : "In Stock";
                table.AddCell(status);
            }

            document.Add(table);
            document.Add(Chunk.NEWLINE);

            var lowStock = products.Count(p => p.StockQty <= p.MinimumStock);
            var outOfStock = products.Count(p => p.StockQty == 0);

            document.Add(new Paragraph($"Total Products: {products.Count}", normalFont));
            document.Add(new Paragraph($"Low Stock: {lowStock}", normalFont));
            document.Add(new Paragraph($"Out of Stock: {outOfStock}", normalFont));

            document.Close();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateTopSellingProductsReportAsync(int topCount = 10)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            document.Add(new Paragraph($"Top {topCount} Selling Products", titleFont));
            document.Add(Chunk.NEWLINE);

            var topProducts = await _context.SaleDetails
                .Include(sd => sd.Product)
                .GroupBy(sd => sd.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(sd => sd.Quantity),
                    Revenue = g.Sum(sd => sd.Total)
                })
                .OrderByDescending(g => g.Quantity)
                .Take(topCount)
                .ToListAsync();

            var table = new PdfPTable(4);
            table.WidthPercentage = 100;

            table.AddCell("Product");
            table.AddCell("Quantity Sold");
            table.AddCell("Revenue");
            table.AddCell("Rank");

            int rank = 1;
            foreach (var item in topProducts)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                table.AddCell(product?.ProductName ?? "N/A");
                table.AddCell(item.Quantity.ToString());
                table.AddCell($"Rs. {item.Revenue:N2}");
                table.AddCell(rank.ToString());
                rank++;
            }

            document.Add(table);
            document.Close();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateProfitReportAsync(DateTime startDate, DateTime endDate)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            document.Add(new Paragraph($"Profit Report - {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", titleFont));
            document.Add(Chunk.NEWLINE);

            var sales = await _context.Sales
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status && s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();

            decimal totalRevenue = 0;
            decimal totalCost = 0;
            decimal totalProfit = 0;

            var table = new PdfPTable(4);
            table.WidthPercentage = 100;

            table.AddCell("Date");
            table.AddCell("Revenue");
            table.AddCell("Cost");
            table.AddCell("Profit");

            var salesByDate = sales.GroupBy(s => s.SaleDate.Date).OrderBy(g => g.Key);

            foreach (var daySales in salesByDate)
            {
                decimal dayRevenue = daySales.Sum(s => s.GrandTotal);
                decimal dayCost = 0;

                foreach (var sale in daySales)
                {
                    foreach (var detail in sale.SaleDetails)
                    {
                        if (detail.Product != null)
                        {
                            dayCost += detail.Product.PurchasePrice * detail.Quantity;
                        }
                    }
                }

                decimal dayProfit = dayRevenue - dayCost;

                table.AddCell(daySales.Key.ToString("yyyy-MM-dd"));
                table.AddCell($"Rs. {dayRevenue:N2}");
                table.AddCell($"Rs. {dayCost:N2}");
                table.AddCell($"Rs. {dayProfit:N2}");

                totalRevenue += dayRevenue;
                totalCost += dayCost;
                totalProfit += dayProfit;
            }

            document.Add(table);
            document.Add(Chunk.NEWLINE);

            document.Add(new Paragraph($"Total Revenue: Rs. {totalRevenue:N2}", titleFont));
            document.Add(new Paragraph($"Total Cost: Rs. {totalCost:N2}", normalFont));
            document.Add(new Paragraph($"Total Profit: Rs. {totalProfit:N2}", titleFont));

            document.Close();
            return memoryStream.ToArray();
        }
    }
}
