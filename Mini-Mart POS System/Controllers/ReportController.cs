using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartPOS.Services;

namespace MiniMartPOS.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ISaleService _saleService;

        public ReportController(IReportService reportService, ISaleService saleService)
        {
            _reportService = reportService;
            _saleService = saleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DailySales()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DailySales(DateTime date)
        {
            var pdfData = await _reportService.GenerateDailySalesReportAsync(date);
            return File(pdfData, "application/pdf", $"DailySales_{date:yyyyMMdd}.pdf");
        }

        public IActionResult MonthlySales()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MonthlySales(int month, int year)
        {
            var pdfData = await _reportService.GenerateMonthlySalesReportAsync(month, year);
            return File(pdfData, "application/pdf", $"MonthlySales_{year}{month:D2}.pdf");
        }

        public async Task<IActionResult> Stock()
        {
            var pdfData = await _reportService.GenerateStockReportAsync();
            return File(pdfData, "application/pdf", "StockReport.pdf");
        }

        public IActionResult TopSelling()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TopSelling(int topCount = 10)
        {
            var pdfData = await _reportService.GenerateTopSellingProductsReportAsync(topCount);
            return File(pdfData, "application/pdf", $"TopSelling_{topCount}.pdf");
        }

        public IActionResult Profit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profit(DateTime startDate, DateTime endDate)
        {
            var pdfData = await _reportService.GenerateProfitReportAsync(startDate, endDate);
            return File(pdfData, "application/pdf", $"ProfitReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }

        public IActionResult SalesHistory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalesHistory(DateTime startDate, DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return PartialView("_SalesHistoryTable", sales);
        }
    }
}
