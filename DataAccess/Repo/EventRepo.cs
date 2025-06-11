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
using Amazon.Runtime.Internal.Util;


namespace DataAccess.Repo
{
    public class EventRepo:IEventRepo
    {
        private AppDbContext _context;
        private FilesService _files;

        public EventRepo(AppDbContext context, FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task Add(Event events)
        {
            await _context.events.AddAsync(events);

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
            
            if (deleteImage != null )
            {
                await _files.DeleteFileByUrlAsync(order.Image);
                
            }
            if (order != null)
            {
                
                    _context.events.Remove(order);
                    await _context.SaveChangesAsync();
                
            }
            else
            {
                // Thêm log hoặc xử lý nếu không tìm thấy đối tượng
                throw new Exception($"Order with ID {id} not found.");
            }

        }


        public async Task<IEnumerable<Event>> GetAll()
        {
            return await _context.events.Include(x => x.Hastag).Include(x=>x.Museum).ToListAsync();
        }

        public async Task<Event> GetById(int id)
        {
            var events = await _context.events.Include(x => x.Hastag).Include(x=>x.Museum).FirstOrDefaultAsync(x => x.Id == id);
            if (events == null)
            {
                return null;
            }

            return events;
        }

        public async Task Update(Event events)
        {
            var exisItem = await GetById(events.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(events);
            }
            else
            {
                await _context.events.AddAsync(events);
            }
            await _context.SaveChangesAsync();
        }

        

       public async Task<IEnumerable<Event>> GetAllByHashtag(string hastag)
        {
            hastag = hastag.ToLower(); // Chuyển đổi đầu vào để so sánh không phân biệt hoa/thường

            return await _context.events
                .Where(x => x.Hastag.Any(h => h.Hashtag.ToLower() == hastag))
                .Include(x => x.Hastag).Include(x=>x.Museum)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAllByMuseumId(int museumId)
        {
            return await _context.events.Include(x => x.Hastag).Include(x => x.Museum).Where(o=>o.MuseumId==museumId).ToListAsync();
        }
    }
}
