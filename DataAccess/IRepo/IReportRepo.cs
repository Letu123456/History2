using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IReportRepo
    {
        Task<IEnumerable<Report>> GetAll();
        Task<Report> GetById(int id);
        Task Add(Report report);
        Task Update(Report report);
        Task Delete(int id);

    }
}
