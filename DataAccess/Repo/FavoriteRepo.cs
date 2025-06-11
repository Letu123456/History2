using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections;

namespace DataAccess.Repo
{
    public class FavoriteRepo:IFavoriteRepo
    {
        private AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public FavoriteRepo(AppDbContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Add(Favorite favorite)
        {
            await _context.favorites.AddAsync(favorite);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var favorite = await GetById(id);
            if (favorite != null)
            {
                _context.favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Favorite>> GetAll()
        {
            return await _context.favorites.ToListAsync();
        }

        public async Task<Favorite> GetById(int id)
        {
            var favorite = await _context.favorites.FirstOrDefaultAsync(o => o.Id == id);
            if (favorite == null)
            {
                return null;
            }

            return favorite;
        }

        public async Task<IEnumerable<Favorite>> GetByUserId(string userId)
        {
            

            User user = await _userManager.FindByIdAsync(userId);

            return await _context.favorites.Where(f => f.UserId == user.Id).ToListAsync();
        }

        public async Task Update(Favorite favorite)
        {
            var exisItem = await GetById(favorite.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(favorite);
            }
            else
            {
                await _context.favorites.AddAsync(favorite);
            }
            await _context.SaveChangesAsync();
        }



    }
}
