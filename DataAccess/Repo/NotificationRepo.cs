using Business.Model;
using Business;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public NotificationRepo(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Add(Notification appli)
        {
            using var _context = _contextFactory.CreateDbContext();
            await _context.notifications.AddAsync(appli);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            using var _context = _contextFactory.CreateDbContext();
            var appli = await GetById(id);
            if (appli != null)
            {
                _context.notifications.Remove(appli);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Notification>> GetAll()
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.notifications.Include(x=>x.ReceiverNoti).Include(x=>x.SenderNotiId).Include(x=>x.Blog).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllByUserId(string userId)
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.notifications.Where(o=>o.SenderNotiId==userId).Include(x => x.ReceiverNoti).Include(x => x.SenderNotiId).Include(x => x.Blog).ToListAsync();
        }

        public async Task<Notification> GetById(int id)
        {
            using var _context = _contextFactory.CreateDbContext();
            var appli = await _context.notifications.FirstOrDefaultAsync(o => o.Id == id);
            if (appli == null)
            {
                return null;
            }

            return appli;
        }

        public async Task Update(Notification appli)
        {
            using var _context = _contextFactory.CreateDbContext();
            var exisItem = await GetById(appli.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(appli);
            }
            else
            {
                await _context.notifications.AddAsync(appli);
            }
            await _context.SaveChangesAsync();
        }
    }
}
