using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IRepliCommentRepo
    {
        Task<IEnumerable<RepliComment>> GetAll();
        Task<RepliComment> GetById(int id);
        Task Add(RepliComment repli);
        Task Update(RepliComment repli);
        Task Delete(int id);
        Task<IEnumerable<RepliComment>> GetAllByCommentId(int commentId);
    }
}
