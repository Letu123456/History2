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
    public class ShippingInforRepo:IShippingInforRepo
    {
        private AppDbContext _context;

        public ShippingInforRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(ShippingInfor comment)
        {
            await _context.shippingInfors.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.shippingInfors.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ShippingInfor>> GetAll()
        {
            return await _context.shippingInfors.ToListAsync();
        }

        public async Task<ShippingInfor> GetById(int id)
        {
            var comment = await _context.shippingInfors.FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(ShippingInfor comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.shippingInfors.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShippingInfor>> GetAllByOrderId(int orderId)
        {
            return await _context.shippingInfors.Where(x => x.OrderId == orderId).ToListAsync();
        }
    }
}
