using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICartItemRepo
    {
        Task<IEnumerable<CartItem>> GetAll();
        Task<IEnumerable<CartItem>> GetAllByCartId(int cartId);
        Task<CartItem> GetById(int id);
        Task<CartItem> FindByCartIdAndProductId(int cartId, int productId);
        Task Add(CartItem blog);
        Task Update(CartItem blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
        
    }
}
