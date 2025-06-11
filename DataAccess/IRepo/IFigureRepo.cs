using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IFigureRepo
    {
        Task<IEnumerable<Figure>> GetAll();
        Task<Figure> GetById(int id);
        Task Add(Figure figure);
        Task Update(Figure figure);
        Task Delete(int id);
        Task<IEnumerable<Figure>> GetAllByCategory(string categoryName);
        Task<Figure> GetByName(string figureName);
    }
}
