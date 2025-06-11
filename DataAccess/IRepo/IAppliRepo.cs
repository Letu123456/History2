using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IAppliRepo
    {
        Task<IEnumerable<Appli>> GetAll();
        Task<Appli> GetById(int id);
        Task Add(Appli appli);
        Task Update(Appli appli);
        Task Delete(int id);
    }
}
