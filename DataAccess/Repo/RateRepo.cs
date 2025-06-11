using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using Business.Migrations;

namespace DataAccess.Repo
{
    public class RateRepo : IRateRepo
    {
        private AppDbContext _context;

        public RateRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Rate rate)
        {
            await _context.rates.AddAsync(rate);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var rate = await GetById(id);
            if (rate != null)
            {
                _context.rates.Remove(rate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Rate>> GetAll()
        {
            return await _context.rates.Include(x => x.User).ToListAsync();
        }

        public async Task<Rate> GetById(int id)
        {
            var rate = await _context.rates.Include(x => x.User).FirstOrDefaultAsync(o => o.Id == id);
            if (rate == null)
            {
                return null;
            }

            return rate;
        }

        public async Task Update(Rate rate)
        {
            var exisItem = await GetById(rate.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(rate);
            }
            else
            {
                await _context.rates.AddAsync(rate);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Rate>> GetAllByBlogId(int blogId)
        {
            return await _context.rates.Where(x => x.BlogId == blogId).Include(x => x.User).ToListAsync();
        }
    }
}
