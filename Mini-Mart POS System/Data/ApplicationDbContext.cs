using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Models;

namespace MiniMartPOS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Backup> Backups { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product configuration
            builder.Entity<Product>()
                .HasIndex(p => p.Barcode)
                .IsUnique();

            // Sale configuration
            builder.Entity<Sale>()
                .HasIndex(s => s.InvoiceNumber)
                .IsUnique();

            // Purchase configuration
            builder.Entity<Purchase>()
                .HasIndex(p => p.PurchaseNumber)
                .IsUnique();

            // Seed initial categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryName = "Beverages", Description = "Soft drinks, juices, water", CreatedDate = DateTime.Now },
                new Category { Id = 2, CategoryName = "Snacks", Description = "Chips, biscuits, cookies", CreatedDate = DateTime.Now },
                new Category { Id = 3, CategoryName = "Dairy", Description = "Milk, cheese, yogurt", CreatedDate = DateTime.Now },
                new Category { Id = 4, CategoryName = "Rice & Grains", Description = "Rice, wheat, flour", CreatedDate = DateTime.Now },
                new Category { Id = 5, CategoryName = "Cosmetics", Description = "Beauty products", CreatedDate = DateTime.Now },
                new Category { Id = 6, CategoryName = "Household", Description = "Cleaning supplies", CreatedDate = DateTime.Now },
                new Category { Id = 7, CategoryName = "Personal Care", Description = "Soap, shampoo, toothpaste", CreatedDate = DateTime.Now },
                new Category { Id = 8, CategoryName = "Frozen Foods", Description = "Frozen items", CreatedDate = DateTime.Now }
            );
        }
    }
}
