using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryVideoRepo
    {
        Task<IEnumerable<CategoryVideo>> GetAll();
        Task<CategoryVideo> GetById(int id);
        Task Add(CategoryVideo cateArt);
        Task Update(CategoryVideo cateArt);
        Task Delete(int id);
    }
}
