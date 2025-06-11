using Business;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repo
{
    public class CategoryProductRepo : ICategoryProductRepo
    {
        private readonly AppDbContext _context;

        public CategoryProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryProduct>> GetAllCategoriesAsync()
        {
            return await _context.categoryProducts.Include(c => c.Products).ToListAsync();
        }

        public async Task<CategoryProduct?> GetCategoryByIdAsync(int id)
        {
            return await _context.categoryProducts.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CategoryProduct> CreateCategoryAsync(CategoryProduct category)
        {
            _context.categoryProducts.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryProduct?> UpdateCategoryAsync(int id, CategoryProduct categoryUpdate)
        {
            var category = await _context.categoryProducts.FindAsync(id);
            if (category == null) return null;

            category.Name = categoryUpdate.Name;
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.categoryProducts.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;

            if (category.Products != null && category.Products.Any())
                return false;

            _context.categoryProducts.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
