using Business.Model;
using Business;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Service;

namespace DataAccess.Repo
{
    public class ArtifactRepo:IArtifactRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public ArtifactRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Artifact artifact)
        {
           await _context.artifact.AddAsync(artifact);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var artifact = await GetById(id);

            var deleteImage = await _files.GetImageByUrlAsync(artifact.Image);
            var deletePodcast = await _files.GetImageByUrlAsync(artifact.Podcast);
            if(deleteImage != null && deletePodcast != null)
            {
                await _files.DeleteFileByUrlAsync(artifact.Image);
                await _files.DeleteFileByUrlAsync(artifact.Podcast);
            }

            if (artifact != null)
            {

                
                
                    _context.artifact.Remove(artifact);
                    await _context.SaveChangesAsync();
                
            }
            else
            {
                // Thêm log hoặc xử lý nếu không tìm thấy đối tượng
                throw new Exception($"Artifact with ID {id} not found.");
            }
        }

        public async Task<IEnumerable<Artifact>> GetAll()
        {
            return await _context.artifact.Include(x=>x.Images).Include(x=>x.CategoryArtifacts).ToListAsync();
        }

        public async Task<IEnumerable<Artifact>> GetAllByCategory(string categoryName)
        {
            var artifact = await _context.artifact.Include(x => x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Historical).Include(x=>x.Museum).Include(x => x.CategoryArtifacts).Where(o => o.CategoryArtifacts.Name == categoryName).ToListAsync();
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }
        public async Task<IEnumerable<Artifact>> GetAllByMuseumId(int museumId)
        {
            var artifact = await _context.artifact.Include(x => x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Historical).Include(x => x.Museum).Include(x => x.CategoryArtifacts).Where(o => o.MuseumId == museumId).ToListAsync();
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }

        public async Task<Artifact> GetById(int id)
        {
            var artifact = await _context.artifact.Include(x=>x.Images).Include(x => x.ArtifactHistoricals).ThenInclude(a => a.Historical).Include(x=>x.Museum).Include(x => x.CategoryArtifacts).FirstOrDefaultAsync(o => o.Id == id);
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }

        public async Task Update(Artifact artifact)
        {
            var exisItem = await GetById(artifact.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(artifact);
            }
            else
            {
               await _context.artifact.AddAsync(artifact);
            }
            await _context.SaveChangesAsync();
        }
    }
}
