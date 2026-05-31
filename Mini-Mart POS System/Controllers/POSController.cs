using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Models;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize]
    public class POSController : Controller
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly ICustomerService _customerService;

        public POSController(
            IProductService productService,
            ISaleService saleService,
            ICustomerService customerService)
        {
            _productService = productService;
            _saleService = saleService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            var viewModel = new POSViewModel
            {
                CartItems = new List<CartItem>(),
                Subtotal = 0,
                Discount = 0,
                Tax = 0,
                GrandTotal = 0
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ScanBarcode(string barcode)
        {
            var product = await _productService.GetProductByBarcodeAsync(barcode);
            
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            if (product.StockQty <= 0)
            {
                return Json(new { success = false, message = "Product out of stock" });
            }

            return Json(new { 
                success = true, 
                product = new {
                    id = product.Id,
                    barcode = product.Barcode,
                    name = product.ProductName,
                    price = product.SellingPrice,
                    stock = product.StockQty
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            if (item.Quantity > product.StockQty)
            {
                return Json(new { success = false, message = "Insufficient stock" });
            }

            return Json(new { 
                success = true, 
                item = new {
                    productId = product.Id,
                    productName = product.ProductName,
                    quantity = item.Quantity,
                    unitPrice = product.SellingPrice,
                    total = item.Quantity * product.SellingPrice
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteSale([FromBody] SaleRequest request)
        {
            try
            {
                var userId = User.Identity?.Name ?? "";
                
                var sale = new Sale
                {
                    UserId = userId,
                    CustomerId = request.CustomerId,
                    Subtotal = request.Subtotal,
                    Discount = request.Discount,
                    Tax = request.Tax,
                    GrandTotal = request.GrandTotal,
                    PaidAmount = request.PaidAmount,
                    PaymentMethod = request.PaymentMethod,
                    Notes = request.Notes
                };

                var saleDetails = request.CartItems.Select(ci => new SaleDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    Discount = ci.Discount,
                    Total = ci.Total
                }).ToList();

                var completedSale = await _saleService.CreateSaleAsync(sale, saleDetails);

                return Json(new { 
                    success = true, 
                    invoiceNumber = completedSale.InvoiceNumber,
                    saleId = completedSale.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        public async Task<IActionResult> PrintReceipt(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            // Generate receipt HTML for printing
            return View("Receipt", sale);
        }
    }
}
