using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Backup
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string? FileName { get; set; }
        
        [Required]
        [StringLength(500)]
        public string? FilePath { get; set; }
        
        public DateTime BackupDate { get; set; } = DateTime.Now;
        
        [Required]
        public long FileSize { get; set; }
        
        [Required]
        public string? BackupType { get; set; } // Automatic, Manual
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        public bool Status { get; set; } = true;
    }
}
