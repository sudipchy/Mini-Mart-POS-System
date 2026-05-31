using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? PurchaseNumber { get; set; }
        
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        
        public int SupplierId { get; set; }
        
        public Supplier? Supplier { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PaidAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal DueAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool Status { get; set; } = true;
        
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
