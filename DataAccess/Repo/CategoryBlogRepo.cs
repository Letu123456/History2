using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class CategoryBlogRepo:ICategoryBlogRepo
    {
        private AppDbContext _context;

        public CategoryBlogRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CategoryBlog cateBlog)
        {
            await _context.categoryBlogs.AddAsync(cateBlog);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateBlog = await GetById(id);
            if (cateBlog != null)
            {
                _context.categoryBlogs.Remove(cateBlog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryBlog>> GetAll()
        {
            return await _context.categoryBlogs.ToListAsync();
        }

        public async Task<CategoryBlog> GetById(int id)
        {
            var cateBlog = await _context.categoryBlogs.FirstOrDefaultAsync(o => o.Id == id);
            if (cateBlog == null)
            {
                return null;
            }

            return cateBlog;
        }

        public async Task Update(CategoryBlog cateBlog)
        {
            var exisItem = await GetById(cateBlog.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateBlog);
            }
            else
            {
                await _context.categoryBlogs.AddAsync(cateBlog);
            }
            await _context.SaveChangesAsync();
        }
    }
}
