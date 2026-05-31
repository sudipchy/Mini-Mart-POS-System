using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class SaleDetail
    {
        public int Id { get; set; }
        
        public int SaleId { get; set; }
        
        public Sale? Sale { get; set; }
        
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
    }
}
