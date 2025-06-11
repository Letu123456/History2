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
    public class VideoRepo:IVideoRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public VideoRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Video blogs)
        {
            await _context.videos.AddAsync(blogs);

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
            var blogs = await GetById(id);
            if (blogs == null)
            {
                throw new Exception($"Blog with ID {id} not found.");
            }

            // Delete related Rates records
            var relatedRates = await _context.commentVideos.Where(r => r.VideoId == id).ToListAsync();
            if (relatedRates.Any())
            {
                _context.commentVideos.RemoveRange(relatedRates);
            }

            // Delete associated image, if any
            if (!string.IsNullOrEmpty(blogs.VideoClip))
            {
                var deleteImage = await _files.GetImageByUrlAsync(blogs.VideoClip);
                if (deleteImage != null)
                {
                    await _files.DeleteFileByUrlAsync(blogs.VideoClip);
                }
            }

            // Delete the blog
            _context.videos.Remove(blogs);
            await _context.SaveChangesAsync();

        }


        public async Task<IEnumerable<Video>> GetAll()
        {
            return await _context.videos.Include(x => x.CategoryVideo).Include(x => x.User).ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetAllByCategory(int categoryId)
        {
            var artifact = await _context.videos.Include(x => x.CategoryVideo).Include(x => x.User).Where(o => o.CategoryVideo.Id == categoryId).ToListAsync();
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }

        public async Task<Video> GetById(int id)
        {
            var blogs = await _context.videos.Include(x => x.CategoryVideo).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (blogs == null)
            {
                return null;
            }

            return blogs;
        }

        public async Task Update(Video blogs)
        {
            var exisItem = await GetById(blogs.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(blogs);
            }
            else
            {
                await _context.videos.AddAsync(blogs);
            }
            await _context.SaveChangesAsync();
        }

        
    }
}
