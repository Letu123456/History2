using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IRateRepo
    {
        Task<IEnumerable<Rate>> GetAll();
        Task<Rate> GetById(int id);
        Task Add(Rate rate);
        Task Update(Rate rate);
        Task Delete(int id);
        Task<IEnumerable<Rate>> GetAllByBlogId(int blogId);
    }
}
