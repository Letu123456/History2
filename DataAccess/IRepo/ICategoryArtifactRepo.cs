using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryArtifactRepo
    {
        Task<IEnumerable<CategoryArtifact>> GetAll();
        Task<CategoryArtifact> GetById(int id);
        Task Add(CategoryArtifact cateArt);
        Task Update(CategoryArtifact cateArt);
        Task Delete(int id);
    }
}
