using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface INotificationRepo
    {
        Task<IEnumerable<Notification>> GetAll();
        Task<IEnumerable<Notification>> GetAllByUserId(string userId);
        Task<Notification> GetById(int id);
        Task Add(Notification notify);
        Task Update(Notification notify);
        Task Delete(int id);
    }
}
