using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IOrderItemRepo
    {
        Task<IEnumerable<OrderItem>> GetAll();
        Task<IEnumerable<OrderItem>> GetAllByOrderId(int orderId);
        Task<OrderItem> GetById(int id);
        Task Add(OrderItem blog);
        Task Update(OrderItem blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
       
    }
}
