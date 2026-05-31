using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Customer
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        public int LoyaltyPoints { get; set; } = 0;
        
        public decimal OutstandingBalance { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
