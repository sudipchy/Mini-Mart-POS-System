using Microsoft.AspNetCore.Identity;

namespace MiniMartPOS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
