using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;
using OfficeOpenXml;

namespace MiniMartPOS.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Status)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Barcode == barcode && p.Status);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            // Check if barcode already exists
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == product.Barcode);
            
            if (existing != null)
            {
                throw new InvalidOperationException("Product with this barcode already exists");
            }

            product.DateAdded = DateTime.Now;
            product.Status = true;
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Product not found");
            }

            existing.Barcode = product.Barcode;
            existing.ProductName = product.ProductName;
            existing.CategoryId = product.CategoryId;
            existing.PurchasePrice = product.PurchasePrice;
            existing.SellingPrice = product.SellingPrice;
            existing.StockQty = product.StockQty;
            existing.MinimumStock = product.MinimumStock;
            existing.SupplierId = product.SupplierId;
            existing.ExpiryDate = product.ExpiryDate;
            existing.Status = product.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status && p.StockQty <= p.MinimumStock)
                .OrderBy(p => p.StockQty)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Status && 
                    (p.ProductName.Contains(searchTerm) || 
                     p.Barcode.Contains(searchTerm) ||
                     p.Category.CategoryName.Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task ImportProductsFromExcelAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var barcode = worksheet.Cells[row, 1].Text;
                    var productName = worksheet.Cells[row, 2].Text;
                    var categoryName = worksheet.Cells[row, 3].Text;
                    var purchasePrice = decimal.Parse(worksheet.Cells[row, 4].Text);
                    var sellingPrice = decimal.Parse(worksheet.Cells[row, 5].Text);
                    var stockQty = int.Parse(worksheet.Cells[row, 6].Text);
                    var minimumStock = int.Parse(worksheet.Cells[row, 7].Text);

                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.CategoryName == categoryName);

                    if (category == null)
                    {
                        category = new Category
                        {
                            CategoryName = categoryName,
                            CreatedDate = DateTime.Now,
                            Status = true
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    var product = new Product
                    {
                        Barcode = barcode,
                        ProductName = productName,
                        CategoryId = category.Id,
                        PurchasePrice = purchasePrice,
                        SellingPrice = sellingPrice,
                        StockQty = stockQty,
                        MinimumStock = minimumStock,
                        DateAdded = DateTime.Now,
                        Status = true
                    };

                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<byte[]> ExportProductsToExcelAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                
                worksheet.Cells[1, 1].Value = "Barcode";
                worksheet.Cells[1, 2].Value = "Product Name";
                worksheet.Cells[1, 3].Value = "Category";
                worksheet.Cells[1, 4].Value = "Purchase Price";
                worksheet.Cells[1, 5].Value = "Selling Price";
                worksheet.Cells[1, 6].Value = "Stock Qty";
                worksheet.Cells[1, 7].Value = "Minimum Stock";
                worksheet.Cells[1, 8].Value = "Supplier";
                worksheet.Cells[1, 9].Value = "Date Added";

                var products = await GetAllProductsAsync();
                int row = 2;

                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Barcode;
                    worksheet.Cells[row, 2].Value = product.ProductName;
                    worksheet.Cells[row, 3].Value = product.Category?.CategoryName;
                    worksheet.Cells[row, 4].Value = product.PurchasePrice;
                    worksheet.Cells[row, 5].Value = product.SellingPrice;
                    worksheet.Cells[row, 6].Value = product.StockQty;
                    worksheet.Cells[row, 7].Value = product.MinimumStock;
                    worksheet.Cells[row, 8].Value = product.Supplier?.SupplierName;
                    worksheet.Cells[row, 9].Value = product.DateAdded.ToString("yyyy-MM-dd");
                    row++;
                }

                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}
