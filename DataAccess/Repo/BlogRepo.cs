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
    public class BlogRepo:IBlogRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public BlogRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Blog blogs)
        {
            await _context.blogs.AddAsync(blogs);

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
            var relatedRates = await _context.rates.Where(r => r.BlogId == id).ToListAsync();
            if (relatedRates.Any())
            {
                _context.rates.RemoveRange(relatedRates);
            }

            // Delete associated image, if any
            if (!string.IsNullOrEmpty(blogs.Image))
            {
                var deleteImage = await _files.GetImageByUrlAsync(blogs.Image);
                if (deleteImage != null)
                {
                    await _files.DeleteFileByUrlAsync(blogs.Image);
                }
            }

            // Delete the blog
            _context.blogs.Remove(blogs);
            await _context.SaveChangesAsync();

        }


        public async Task<IEnumerable<Blog>> GetAll()
        {
            return await _context.blogs.Include(x=>x.CategoryBlog).Include(x => x.HastagOfBlog).Include(x => x.User).ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetAllByUserId(string userId)
        {
            return await _context.blogs.Where(x=> x.UserId==userId).Include(x => x.CategoryBlog).Include(x => x.HastagOfBlog).Include(x => x.User).ToListAsync();
        }

        public async Task<Blog> GetById(int id)
        {
            var blogs = await _context.blogs.Include(x => x.CategoryBlog).Include(x => x.HastagOfBlog).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (blogs == null)
            {
                return null;
            }

            return blogs;
        }

        public async Task Update(Blog blogs)
        {
            var exisItem = await GetById(blogs.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(blogs);
            }
            else
            {
                await _context.blogs.AddAsync(blogs);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Blog>> GetAllByHashtag(string hastag)
        {
            hastag = hastag.ToLower(); // Chuyển đổi đầu vào để so sánh không phân biệt hoa/thường

            return await _context.blogs
                .Where(x => x.HastagOfBlog.Any(h => h.Hashtag.ToLower() == hastag))
                .Include(x => x.HastagOfBlog).Include(x => x.User).Include(x => x.CategoryBlog)
                .ToListAsync();
        }


    }
}
