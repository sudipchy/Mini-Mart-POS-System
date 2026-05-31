using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models.ViewModels
{
    public class StockTransactionViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? Notes { get; set; }
    }

    public class StockAdjustmentViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int NewQuantity { get; set; }

        public string? Notes { get; set; }
    }
}
