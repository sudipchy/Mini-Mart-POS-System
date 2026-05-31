using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? Barcode { get; set; }
        
        [Required]
        [StringLength(200)]
        public string? ProductName { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQty { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStock { get; set; }
        
        public int? SupplierId { get; set; }
        
        public Supplier? Supplier { get; set; }
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        public DateTime? ExpiryDate { get; set; }
        
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    }
}
