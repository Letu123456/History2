using Business.Model;
using Business;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Business.DTO;

namespace DataAccess.Repo
{
    public class OrderRepo:IOrderRepo
    {
        private AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Order comment)
        {
            await _context.orders.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.orders.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
           
            return await _context.orders.Include(x => x.OrderItems).ToListAsync();
        }

        
        public async Task<Order> GetById(int id)
        {
            var comment = await _context.orders.Include(x => x.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(Order comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.orders.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllByUserId(string userId)
        {
            return await _context.orders.Where(x => x.UserId == userId).Include(x => x.OrderItems).ToListAsync();
        }
    }
}
