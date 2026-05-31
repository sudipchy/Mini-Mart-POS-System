using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class InventoryLog
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        [Required]
        [StringLength(20)]
        public string? TransactionType { get; set; } // Stock In, Stock Out, Sale, Purchase
        
        public int Quantity { get; set; }
        
        public int PreviousStock { get; set; }
        
        public int NewStock { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
