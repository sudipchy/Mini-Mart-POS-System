using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;

        public HomeController(IProductService productService, ISaleService saleService)
        {
            _productService = productService;
            _saleService = saleService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TodaySales = await _saleService.GetTodaySalesAsync(),
                TodayProfit = await _saleService.GetTodayProfitAsync(),
                TotalProducts = (await _productService.GetAllProductsAsync()).Count(),
                LowStockAlerts = (await _productService.GetLowStockProductsAsync()).Count(),
                RecentTransactions = (await _saleService.GetRecentTransactionsAsync(5)).ToList(),
                LowStockProducts = (await _productService.GetLowStockProductsAsync()).Take(5).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
