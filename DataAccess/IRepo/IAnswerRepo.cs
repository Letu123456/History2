using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IAnswerRepo
    {
        Task<IEnumerable<Answer>> GetAllAsync();
        Task<Answer?> GetByIdAsync(int id);
        Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId);
        Task AddAsync(Answer answer);
        Task UpdateAsync(Answer answer);
        Task DeleteAsync(int id);
    }

}
