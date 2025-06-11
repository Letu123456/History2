using Business;

using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repo
{
    public class HistoricalRepo : IHistoricalRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public HistoricalRepo(AppDbContext context,FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Historical history)
        {
           await  _context.historicals.AddAsync(history);

            await _context.SaveChangesAsync();
            //return history;

            //try
            //{

            //    _context.historicals.Add(history);
            //    _context.SaveChanges();
                
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
        }

        public async Task Delete(int id)
        {
            var order = await GetById(id);

            var deleteImage = await _files.GetImageByUrlAsync(order.Image);
            var deletePodcast = await _files.GetImageByUrlAsync(order.Podcast);
            var deleteVideo = await _files.GetImageByUrlAsync(order.Video);

            if (deleteImage != null && deletePodcast != null&&deleteVideo!=null)
            {
                await _files.DeleteFileByUrlAsync(order.Image);
                await _files.DeleteFileByUrlAsync(order.Podcast);
                await _files.DeleteFileByUrlAsync(order.Video);

            }
            if (order != null)
            {
              
                    _context.historicals.Remove(order);
                    await _context.SaveChangesAsync();
                
            }
            else
            {
                // Thêm log hoặc xử lý nếu không tìm thấy đối tượng
                throw new Exception($"Order with ID {id} not found.");
            }

        }
        

        public async Task<IEnumerable<Historical>> GetAll()
        {
            return await _context.historicals.Include(x => x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Artifact).Include(x => x.HistoricalFigures).ThenInclude(a => a.Figure).ToListAsync();
        }
        public async Task<IEnumerable<Historical>> GetAllByCategory(string categoryName)
        {
            var history = await _context.historicals.Include(x => x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Artifact).Include(x => x.HistoricalFigures).ThenInclude(a => a.Figure).Include(x => x.CategoryHistorical).Where(o => o.CategoryHistorical.Name == categoryName).ToListAsync();
            if (history == null)
            {
                return null;
            }

            return history;
        }
        public async Task<Historical> GetById(int id)
        {
            var history = await _context.historicals.Include(x => x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Artifact).Include(x => x.HistoricalFigures).ThenInclude(a => a.Figure).Include(x => x.CategoryHistorical).FirstOrDefaultAsync(x => x.Id == id);
            if (history == null)
            {
                return null;
            }

            return history;
        }

        public async Task Update(Historical history)
        {
            var exisItem = await GetById(history.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(history);
            }
            else
            {
               await _context.historicals.AddAsync(history);
            }
            await _context.SaveChangesAsync();
        }
    }
}
