using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string? SupplierName { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
