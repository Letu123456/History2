using Business.Model;
using Business;
using DataAccess.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repo
{
    public class MuseumRepo:IMuseumRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public MuseumRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Museum museum)
        {
            await _context.museums.AddAsync(museum);

            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(int id)
        {
            var museum = await GetById(id);
            var deleteImage = await _files.GetImageByUrlAsync(museum.Image);
            var deletePodcast = await _files.GetImageByUrlAsync(museum.Video);
            if (deleteImage != null && deletePodcast != null)
            {
                await _files.DeleteFileByUrlAsync(museum.Image);
                await _files.DeleteFileByUrlAsync(museum.Video);
            }
            if (museum != null)
            {
                
                    _context.museums.Remove(museum);
                    await _context.SaveChangesAsync();
                
            }
            else
            {
                // Thêm log hoặc xử lý nếu không tìm thấy đối tượng
                throw new Exception($"Museum with ID {id} not found.");
            }

        }


        public async Task<IEnumerable<Museum>> GetAll()
        {
            return await _context.museums.Include(x => x.Images).Include(x => x.Artifacts).Include(x=>x.Events).Include(x=>x.User).ToListAsync();
        }

        public async Task<Museum> GetById(int id)
        {
            var museum = await _context.museums.Include(x => x.Images).Include(x => x.Artifacts).Include(x => x.Events).Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (museum == null)
            {
                return null;
            }

            return museum;
        }

        public async Task Update(Museum museum)
        {
            var exisItem = await GetById(museum.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(museum);
            }
            else
            {
                await _context.museums.AddAsync(museum);
            }
            await _context.SaveChangesAsync();
        }
    }
}
