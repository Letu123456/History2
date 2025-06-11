using Business.Model;
using Business;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class CartRepo:ICartRepo
    {
        private AppDbContext _context;

        public CartRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Cart comment)
        {
            await _context.carts.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.carts.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Cart>> GetAll()
        {
            return await _context.carts.Include(x => x.User).Include(x => x.CartItems).ThenInclude(c=>c.Product).ToListAsync();
        }

        public async Task<Cart> GetById(int id)
        {
            var comment = await _context.carts.Include(x => x.User).Include(x => x.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(Cart comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.carts.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cart>> GetAllByUserId(string userId)
        {
            return await _context.carts.Where(x => x.UserId == userId).Include(x => x.User).Include(x => x.CartItems).ThenInclude(c => c.Product).ToListAsync();
        }
    }
}
