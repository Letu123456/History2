using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace DataAccess.IRepo
{
    public interface IHistoricalRepo
    {
        Task<IEnumerable<Historical>> GetAll();
        Task<Historical> GetById(int id);
        Task Add(Historical history);
        Task Update(Historical history);
        Task Delete(int id);
        Task<IEnumerable<Historical>> GetAllByCategory(string categoryName);
    }
}
