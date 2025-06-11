using Business.Model;
using Business;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Business.DTO;

namespace DataAccess.Repo
{
    public class AnswerRepo : IAnswerRepo
    {
        private readonly AppDbContext _context;

        public AnswerRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answer>> GetAllAsync()
        {
            return await _context.answers.Include(a => a.Question).ToListAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.answers.Include(a => a.Question)
                                         .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task AddAsync(Answer answer)
        {
            _context.answers.Add(answer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Answer answer)
        {
            _context.answers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var answer = await _context.answers.FindAsync(id);
            if (answer != null)
            {
                _context.answers.Remove(answer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
