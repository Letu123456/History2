using Business;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repo
{
    public class OrderItemRepo:IOrderItemRepo
    {
        private AppDbContext _context;

        public OrderItemRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(OrderItem comment)
        {
            await _context.orderItems.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.orderItems.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAll()
        {
            return await _context.orderItems.Include(x=>x.Product).ToListAsync();
        }

        public async Task<OrderItem> GetById(int id)
        {
            var comment = await _context.orderItems.Include(x => x.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(OrderItem comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.orderItems.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderId(int orderId)
        {
            return await _context.orderItems.Include(x => x.Product).Where(x => x.OrderId == orderId).ToListAsync();
        }
    }
}
