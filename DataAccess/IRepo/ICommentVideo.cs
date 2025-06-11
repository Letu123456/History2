using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface ICommentVideo
    {
        Task<IEnumerable<CommentVideo>> GetAll();

        Task<CommentVideo> GetById(int id);
        Task Add(CommentVideo cmtVideo);
        Task Update(CommentVideo cmtVideo);
        Task Delete(int id);
        Task<IEnumerable<CommentVideo>> GetAllByVideoId(int videoId);

    }
}
