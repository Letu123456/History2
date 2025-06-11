using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IFavoriteRepo
    {
        Task<IEnumerable<Favorite>> GetAll();
        Task<Favorite> GetById(int id);
        Task<IEnumerable<Favorite>> GetByUserId(string userId);
        Task Add(Favorite favorite);
        Task Update(Favorite favorite);
        Task Delete(int id);
    }
}
