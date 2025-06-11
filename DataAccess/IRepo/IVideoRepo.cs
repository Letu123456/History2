using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IVideoRepo
    {
        Task<IEnumerable<Video>> GetAll();
       
        Task<Video> GetById(int id);
        Task Add(Video video);
        Task Update(Video video);
        Task Delete(int id);
        Task<IEnumerable<Video>> GetAllByCategory(int categoryId);
    }
}
