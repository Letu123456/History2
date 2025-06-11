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
    public class CategoryFigureRepo : ICategoryFigureRepo
    {
        private AppDbContext _context;

        public CategoryFigureRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CategoryFigure cateArtifact)
        {
            await _context.categoryFigures.AddAsync(cateArtifact);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateArtifact = await GetById(id);
            if (cateArtifact != null)
            {
                _context.categoryFigures.Remove(cateArtifact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryFigure>> GetAll()
        {
            return await _context.categoryFigures.ToListAsync();
        }

        public async Task<CategoryFigure> GetById(int id)
        {
            var cateArtifact = await _context.categoryFigures.FirstOrDefaultAsync(o => o.Id == id);
            if (cateArtifact == null)
            {
                return null;
            }

            return cateArtifact;
        }

        public async Task Update(CategoryFigure cateArtifact)
        {
            var exisItem = await GetById(cateArtifact.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateArtifact);
            }
            else
            {
                await _context.categoryFigures.AddAsync(cateArtifact);
            }
            await _context.SaveChangesAsync();
        }
    }
}
