using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Models;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            IEnumerable<Product> products;
            
            if (!string.IsNullOrEmpty(search))
            {
                products = await _productService.SearchProductsAsync(search);
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _supplierService.GetAllSuppliersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.AddProductAsync(product);
                    TempData["Success"] = "Product added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _supplierService.GetAllSuppliersAsync();
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetLowStockProductsAsync();
            return View(products);
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                try
                {
                    await _productService.ImportProductsFromExcelAsync(filePath);
                    TempData["Success"] = "Products imported successfully.";
                    System.IO.File.Delete(filePath);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error importing file: " + ex.Message);
                    System.IO.File.Delete(filePath);
                }
            }

            return View();
        }

        public async Task<IActionResult> Export()
        {
            var excelData = await _productService.ExportProductsToExcelAsync();
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }
    }
}
