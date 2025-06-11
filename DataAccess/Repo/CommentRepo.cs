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
    public class CommentRepo:ICommentRepo
    {
        private AppDbContext _context;

        public CommentRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Comment comment)
        {
            await _context.comments.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await GetById(id);
            if (comment != null)
            {
                _context.comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            return await _context.comments.Include(x => x.User).Include(x => x.RepliComments).ToListAsync();
        }

        public async Task<Comment> GetById(int id)
        {
            var comment = await _context.comments.Include(x => x.User).Include(x => x.RepliComments).FirstOrDefaultAsync(o => o.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task Update(Comment comment)
        {
            var exisItem = await GetById(comment.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(comment);
            }
            else
            {
                await _context.comments.AddAsync(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllByEventId(int eventId)
        {
            return await _context.comments.Where(x =>x.EventId == eventId).Include(x => x.User).Include(x => x.RepliComments).ToListAsync();
        }
    }
}
