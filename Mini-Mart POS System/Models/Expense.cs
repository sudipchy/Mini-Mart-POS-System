using System.ComponentModel.DataAnnotations;

namespace MiniMartPOS.Models
{
    public class Expense
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? ExpenseName { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? Category { get; set; } // Rent, Electricity, Salary, Transport, Other
        
        public DateTime ExpenseDate { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        public bool Status { get; set; } = true;
    }
}
