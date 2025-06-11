using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryBlogRepo
    {
        Task<IEnumerable<CategoryBlog>> GetAll();
        Task<CategoryBlog> GetById(int id);
        Task Add(CategoryBlog cateBlog);
        Task Update(CategoryBlog cateBlog);
        Task Delete(int id);
    }
}
