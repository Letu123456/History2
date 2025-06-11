using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryFigureRepo
    {
        Task<IEnumerable<CategoryFigure>> GetAll();
        Task<CategoryFigure> GetById(int id);
        Task Add(CategoryFigure cateArt);
        Task Update(CategoryFigure cateArt);
        Task Delete(int id);
    }
}
