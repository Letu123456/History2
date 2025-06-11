using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class CommentVideoRepo:ICommentVideo
    {
        private AppDbContext _context;

        public CommentVideoRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CommentVideo comment)
        {
            await _context.commentVideos.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.commentVideos.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CommentVideo>> GetAll()
        {
            return await _context.commentVideos.Include(x => x.User).ToListAsync();
        }

        public async Task<CommentVideo> GetById(int id)
        {
            var comment = await _context.commentVideos.Include(x => x.User).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(CommentVideo comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.commentVideos.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentVideo>> GetAllByVideoId(int videoId)
        {
            return await _context.commentVideos.Where(x => x.VideoId == videoId).Include(x => x.User).ToListAsync();
        }
    }
}
