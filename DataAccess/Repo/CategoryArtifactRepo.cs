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
    public class CategoryArtifactRepo:ICategoryArtifactRepo
    {
        private AppDbContext _context;

        public CategoryArtifactRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CategoryArtifact cateArtifact)
        {
          await  _context.categoryArtifacts.AddAsync(cateArtifact);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateArtifact = await GetById(id);
            if (cateArtifact != null)
            {
                _context.categoryArtifacts.Remove(cateArtifact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryArtifact>> GetAll()
        {
            return await _context.categoryArtifacts.Include(x=>x.CategoryHistorical).ToListAsync();
        }

        public async Task<CategoryArtifact> GetById(int id)
        {
            var cateArtifact = await _context.categoryArtifacts.Include(x => x.CategoryHistorical).FirstOrDefaultAsync(o => o.Id == id);
            if (cateArtifact == null)
            {
                return null;
            }

            return cateArtifact;
        }

        public async Task Update(CategoryArtifact cateArtifact)
        {
            var exisItem = await GetById(cateArtifact.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateArtifact);
            }
            else
            {
               await _context.categoryArtifacts.AddAsync(cateArtifact);
            }
            await _context.SaveChangesAsync();
        }
    }
}
