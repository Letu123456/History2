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
    public class CartItemRepo:ICartItemRepo
    {
        private AppDbContext _context;

        public CartItemRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CartItem comment)
        {
            await _context.cartItems.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.cartItems.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CartItem>> GetAll()
        {
            return await _context.cartItems.Include(x=>x.Product).ToListAsync();
        }

        public async Task<CartItem> GetById(int id)
        {
            var comment = await _context.cartItems.Include(x => x.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(CartItem comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.cartItems.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<CartItem?> FindByCartIdAndProductId(int cartId, int productId)
        {
            return await _context.cartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetAllByCartId(int cartId)
        {
            return await _context.cartItems.Include(x => x.Product).Where(x => x.CartId == cartId).ToListAsync();
        }
    }
}
