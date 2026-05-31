using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        
        public int PurchaseId { get; set; }
        
        public Purchase? Purchase { get; set; }
        
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
        public decimal Total { get; set; }
    }
}
