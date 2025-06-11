using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IMuseumRepo
    {
        Task<IEnumerable<Museum>> GetAll();
        Task<Museum> GetById(int id);
        Task Add(Museum museum);
        Task Update(Museum museum);
        Task Delete(int id);
    }
}
