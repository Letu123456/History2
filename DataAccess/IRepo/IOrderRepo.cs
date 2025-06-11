using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAll();
        Task<IEnumerable<Order>> GetAllByUserId(string userId);
        Task<Order> GetById(int id);
        Task Add(Order blog);
        Task Update(Order blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
        
    }
}
