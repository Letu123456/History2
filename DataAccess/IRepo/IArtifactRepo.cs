using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IArtifactRepo
    {
        Task<IEnumerable<Artifact>> GetAll();
        Task<IEnumerable<Artifact>> GetAllByMuseumId(int museumId);
        Task<Artifact> GetById(int id);
        Task Add(Artifact artifact);
        Task Update(Artifact history);
        Task Delete(int id);
        Task<IEnumerable<Artifact>> GetAllByCategory(string categoryName);

    }
}
