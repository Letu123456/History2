using Business;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repo
{
    public class QuizRepo : IQuizRepo
    {
  
        private readonly AppDbContext _context;

        public QuizRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.quiz.Include(q => q.CategoryHistoricals).Include(q => q.Questions)
                                         .ThenInclude(q => q.Answers)
                                         .ToListAsync();
        }

        public async Task<Quiz?> GetByIdAsync(int id)
        {
            return await _context.quiz.Include(q => q.Questions)
                                         .ThenInclude(q => q.Answers)
                                         .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task AddAsync(Quiz quiz)
        {
            _context.quiz.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            _context.quiz.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var quiz = await _context.quiz.FindAsync(id);
            if (quiz != null)
            {
                _context.quiz.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveUserQuizResultAsync(UserQuizResult result)
        {
            _context.userQuizResults.Add(result);
            await _context.SaveChangesAsync();
        }
    }

}
