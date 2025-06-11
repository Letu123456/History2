using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using DataAccess.Service;

namespace DataAccess.Repo
{
    public class AppliRepo:IAppliRepo
    {
        /*private AppDbContext _context*/
        private readonly AppDbContext _context;
        private readonly FilesService _files;

        public AppliRepo(AppDbContext context,FilesService files)
        {
            _context= context;
            _files= files;
        }

        public async Task Add(Appli appli)
        {
            
            await _context.applis.AddAsync(appli);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            
            var appli = await GetById(id);
            var deleteImage = await _files.GetImageByUrlAsync(appli.Image);
            if (deleteImage != null)
            {
                await _files.DeleteFileByUrlAsync(appli.Image);

            }
            if (appli != null)
            {
                _context.applis.Remove(appli);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Appli>> GetAll()
        {
            
            return await _context.applis.Include(x => x.User).ToListAsync();
        }

        public async Task<Appli> GetById(int id)
        {
           
            var appli = await _context.applis.Include(x => x.User).FirstOrDefaultAsync(o => o.Id == id);
            if (appli == null)
            {
                return null;
            }

            return appli;
        }

        public async Task Update(Appli appli)
        {
            
            var exisItem = await GetById(appli.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(appli);
            }
            else
            {
                await _context.applis.AddAsync(appli);
            }
            await _context.SaveChangesAsync();
        }
    }
}
