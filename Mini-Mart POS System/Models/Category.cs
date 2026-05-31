using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? CategoryName { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
