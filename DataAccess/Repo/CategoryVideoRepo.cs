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
    public class CategoryVideoRepo:ICategoryVideoRepo
    {
        private AppDbContext _context;

        public CategoryVideoRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CategoryVideo cateBlog)
        {
            await _context.categoryVideos.AddAsync(cateBlog);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateBlog = await GetById(id);
            if (cateBlog != null)
            {
                _context.categoryVideos.Remove(cateBlog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryVideo>> GetAll()
        {
            return await _context.categoryVideos.ToListAsync();
        }

        public async Task<CategoryVideo> GetById(int id)
        {
            var cateBlog = await _context.categoryVideos.FirstOrDefaultAsync(o => o.Id == id);
            if (cateBlog == null)
            {
                return null;
            }

            return cateBlog;
        }

        public async Task Update(CategoryVideo cateBlog)
        {
            var exisItem = await GetById(cateBlog.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateBlog);
            }
            else
            {
                await _context.categoryVideos.AddAsync(cateBlog);
            }
            await _context.SaveChangesAsync();
        }
    }
}
