using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IShippingInforRepo
    {
        Task<IEnumerable<ShippingInfor>> GetAll();
        Task<IEnumerable<ShippingInfor>> GetAllByOrderId(int orderId);
        Task<ShippingInfor> GetById(int id);
        Task Add(ShippingInfor blog);
        Task Update(ShippingInfor blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
    }
}
