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
using DataAccess.Service;

namespace DataAccess.Repo
{
    public class QuestionRepo : IQuestionRepo
    {
        private readonly AppDbContext _context;
        private FilesService _files;

        public QuestionRepo(AppDbContext context,FilesService files)
        {
            _context = context;
            _files = files;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.question.Include(q=>q.Quiz).Include(q => q.Answers).ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.question.Include(q => q.Quiz).Include(q => q.Answers)
                                           .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId)
        {
            return await _context.question
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            _context.question.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.question.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.question.FindAsync(id);
            var deleteImage = await _files.GetImageByUrlAsync(question.Image);
            if (deleteImage != null)
            {
                await _files.DeleteFileByUrlAsync(question.Image);

            }
            if (question != null)
            {
                _context.question.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }
}
