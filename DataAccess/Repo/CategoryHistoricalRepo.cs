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
    public class CategoryHistoricalRepo:ICategoryHistoricalRepo
    {
        private AppDbContext _context;

        public CategoryHistoricalRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CategoryHistorical cateHistory)
        {
           await _context.categoryHistoricals.AddAsync(cateHistory);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateHistory = await GetById(id);
            if (cateHistory != null)
            {
                _context.categoryHistoricals.Remove(cateHistory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryHistorical>> GetAll()
        {
            return await _context.categoryHistoricals.ToListAsync();
        }

        public async Task<CategoryHistorical> GetById(int id)
        {
            var cateHistory = await _context.categoryHistoricals.FirstOrDefaultAsync(o => o.Id == id);
            if (cateHistory == null)
            {
                return null;
            }

            return cateHistory;
        }

        public async Task Update(CategoryHistorical cateHistory)
        {
            var exisItem = await GetById(cateHistory.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateHistory);
            }
            else
            {
               await _context.categoryHistoricals.AddAsync(cateHistory);
            }
            await _context.SaveChangesAsync();
        }
    }
}
