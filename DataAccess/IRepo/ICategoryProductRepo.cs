using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICategoryProductRepo
    {
        Task<IEnumerable<CategoryProduct>> GetAllCategoriesAsync();
        Task<CategoryProduct?> GetCategoryByIdAsync(int id);
        Task<CategoryProduct> CreateCategoryAsync(CategoryProduct category);
        Task<CategoryProduct?> UpdateCategoryAsync(int id, CategoryProduct categoryUpdate);
        Task<bool> DeleteCategoryAsync(int id);
    }

}
