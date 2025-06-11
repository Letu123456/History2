using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IMessageRepo
    {
        Task<IEnumerable<Message>> GetAll();
        Task<Message?> GetById(int id);
        Task<IEnumerable<Message>> GetByMessageByUsserId(string sender, string receiver);
        Task Add(Message message);
        Task Update(Message message);
        Task Delete(int id);
    }
}
