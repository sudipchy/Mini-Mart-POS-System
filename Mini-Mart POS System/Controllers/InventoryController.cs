using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Models;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;

        public InventoryController(IInventoryService inventoryService, IProductService productService)
        {
            _inventoryService = inventoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _inventoryService.GetInventoryLogsAsync();
            return View(logs);
        }

        public async Task<IActionResult> StockIn()
        {
            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(StockTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity?.Name ?? "";
                await _inventoryService.StockInAsync(model.ProductId, model.Quantity, userId, model.Notes);
                TempData["Success"] = "Stock added successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View(model);
        }

        public async Task<IActionResult> StockOut()
        {
            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOut(StockTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.Identity?.Name ?? "";
                    await _inventoryService.StockOutAsync(model.ProductId, model.Quantity, userId, model.Notes);
                    TempData["Success"] = "Stock removed successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View(model);
        }

        public async Task<IActionResult> Adjust()
        {
            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjustmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity?.Name ?? "";
                await _inventoryService.AdjustStockAsync(model.ProductId, model.NewQuantity, userId, model.Notes);
                TempData["Success"] = "Stock adjusted successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View(model);
        }

        public async Task<IActionResult> OutOfStock()
        {
            var products = await _inventoryService.GetOutOfStockProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Expiring()
        {
            var products = await _inventoryService.GetExpiringProductsAsync(30);
            return View(products);
        }

        public async Task<IActionResult> ProductLogs(int productId)
        {
            var logs = await _inventoryService.GetInventoryLogsByProductAsync(productId);
            var product = await _productService.GetProductByIdAsync(productId);
            ViewBag.ProductName = product?.ProductName;
            return View(logs);
        }
    }
}
