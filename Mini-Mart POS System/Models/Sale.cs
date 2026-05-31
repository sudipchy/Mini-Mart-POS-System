using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Sale
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }
        
        public DateTime SaleDate { get; set; } = DateTime.Now;
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        public int? CustomerId { get; set; }
        
        public Customer? Customer { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Tax { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal GrandTotal { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PaidAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal ChangeAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool Status { get; set; } = true;
        
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
