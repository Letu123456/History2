using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICommentRepo
    {
        Task<IEnumerable<Comment>> GetAll();
        Task<Comment> GetById(int id);
        Task Add(Comment comment);
        Task Update(Comment comment);
        Task Delete(int id);
        Task<IEnumerable<Comment>> GetAllByEventId(int eventId);
    }
}
