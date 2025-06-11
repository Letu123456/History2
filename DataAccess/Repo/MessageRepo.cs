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
    public class MessageRepo:IMessageRepo
    {
        private readonly AppDbContext _context;

        public MessageRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Message message)
        {

            await _context.messages.AddAsync(message);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {

            var message = await GetById(id);
            if (message != null)
            {
                _context.messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetAll()
        {

            return await _context.messages.Include(x => x.Sender).Include(x=>x.Receiver).ToListAsync();
        }

        public async Task<Message> GetById(int id)
        {

            var message = await _context.messages.Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(o => o.Id == id);
            if (message == null)
            {
                return null;
            }

            return message;
        }

        

        public async Task<IEnumerable<Message>> GetByMessageByUsserId(string senderId, string receiverId)
        {
            return await _context.messages.Include(x => x.Sender).Include(x => x.Receiver).Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId)) // Chat 2 chiều
        .OrderBy(m => m.Timestamp).ToListAsync(); // Sắp xếp theo thời gianToListAsync();
        }

        public async Task Update(Message message)
        {

            var exisItem = await GetById(message.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(message);
            }
            else
            {
                await _context.messages.AddAsync(message);
            }
            await _context.SaveChangesAsync();
        }
    }
}
