using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? Action { get; set; } // Create, Update, Delete, Login, Logout
        
        [Required]
        [StringLength(100)]
        public string? Module { get; set; } // Product, Sale, User, etc.
        
        [StringLength(50)]
        public string? RecordId { get; set; }
        
        [StringLength(1000)]
        public string? OldValues { get; set; }
        
        [StringLength(1000)]
        public string? NewValues { get; set; }
        
        public DateTime ActionDate { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string? IPAddress { get; set; }
    }
}
