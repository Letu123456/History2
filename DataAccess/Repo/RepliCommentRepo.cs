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
    public class RepliCommentRepo:IRepliCommentRepo
    {
        private AppDbContext _context;

        public RepliCommentRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(RepliComment repli)
        {
            await _context.replicomments.AddAsync(repli);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var repli = await GetById(id);
            if (repli != null)
            {
                _context.replicomments.Remove(repli);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RepliComment>> GetAll()
        {
            return await _context.replicomments.Include(x => x.User).ToListAsync();
        }

        public async Task<RepliComment> GetById(int id)
        {
            var repli = await _context.replicomments.Include(x => x.User).FirstOrDefaultAsync(o => o.Id == id);
            if (repli == null)
            {
                return null;
            }

            return repli;
        }

        public async Task Update(RepliComment repli)
        {
            var exisItem = await GetById(repli.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(repli);
            }
            else
            {
                await _context.replicomments.AddAsync(repli);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RepliComment>> GetAllByCommentId(int commentId)
        {
            return await _context.replicomments.Include(x => x.User).Where(x=>x.CommentId==commentId).ToListAsync();
        }
    }
}
