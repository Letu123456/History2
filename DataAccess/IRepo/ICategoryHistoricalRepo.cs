using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryHistoricalRepo
    {
        Task<IEnumerable<CategoryHistorical>> GetAll();
        Task<CategoryHistorical> GetById(int id);
        Task Add(CategoryHistorical cateHistory);
        Task Update(CategoryHistorical cateHistory);
        Task Delete(int id);
    }
}
