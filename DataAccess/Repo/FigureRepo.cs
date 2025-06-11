using Business.Model;
using Business;
using DataAccess.IRepo;
using DataAccess.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class FigureRepo:IFigureRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public FigureRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Figure figure)
        {
            await _context.figures.AddAsync(figure);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var artifact = await GetById(id);

            var deleteImage = await _files.GetImageByUrlAsync(artifact.Image);
            var deletePodcast = await _files.GetImageByUrlAsync(artifact.Podcast);
            if (deleteImage != null && deletePodcast != null)
            {
                await _files.DeleteFileByUrlAsync(artifact.Image);
                await _files.DeleteFileByUrlAsync(artifact.Podcast);
            }
            if (artifact != null)
            {
                
                    _context.figures.Remove(artifact);
                    await _context.SaveChangesAsync();
               
            }
            else
            {
                // Thêm log hoặc xử lý nếu không tìm thấy đối tượng
                throw new Exception($"Artifact with ID {id} not found.");
            }
        }

        public async Task<IEnumerable<Figure>> GetAll()
        {
            return await _context.figures.Include(x => x.Images).Include(x => x.CategoryFigure).ToListAsync();
        }

        public async Task<IEnumerable<Figure>> GetAllByCategory(string categoryName)
        {
            var figure = await _context.figures.Include(x => x.Images).Include(x => x.CategoryFigure).Where(o => o.CategoryFigure.Name == categoryName).ToListAsync();
            if (figure == null)
            {
                return null;
            }

            return figure;
        }

        public async Task<Figure> GetById(int id)
        {
            var artifact = await _context.figures.Include(x => x.Images).Include(x => x.CategoryFigure).FirstOrDefaultAsync(o => o.Id == id);
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }


        public async Task<Figure> GetByName(string figureName)
        {
            var artifact = await _context.figures.Include(x => x.Images).Include(x => x.CategoryFigure).FirstOrDefaultAsync(o => o.Name == figureName );
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }

        public async Task Update(Figure figure)
        {
            
            
            var exisItem = await GetById(figure.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(figure);
            }
            else
            {
                await _context.figures.AddAsync(figure);
            }
            await _context.SaveChangesAsync();
        }

       
    }
}
