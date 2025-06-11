using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICartRepo
    {
        Task<IEnumerable<Cart>> GetAll();
        Task<IEnumerable<Cart>> GetAllByUserId(string userId);
        Task<Cart> GetById(int id);
        Task Add(Cart blog);
        Task Update(Cart blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
        
    }
}
