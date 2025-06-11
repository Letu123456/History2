using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IBlogRepo
    {
        Task<IEnumerable<Blog>> GetAll();
        Task<IEnumerable<Blog>> GetAllByUserId(string userId);
        Task<Blog> GetById(int id);
        Task Add(Blog blog);
        Task Update(Blog blog);
        //Task UpdateIsAccept(Blog blog);
        Task Delete(int id);
        Task<IEnumerable<Blog>> GetAllByHashtag(string hastag);

    }
}
