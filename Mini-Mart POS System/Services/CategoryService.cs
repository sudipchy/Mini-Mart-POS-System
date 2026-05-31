using Microsoft.EntityFrameworkCore;
using MiniMartPOS.Data;
using MiniMartPOS.Models;

namespace MiniMartPOS.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.Status)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            category.CreatedDate = DateTime.Now;
            category.Status = true;
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Category not found");
            }

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.Status = category.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.Status = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
