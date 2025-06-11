using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IEventRepo
    {
        Task<IEnumerable<Event>> GetAll();
        Task<IEnumerable<Event>> GetAllByMuseumId(int museumId);
        Task<Event> GetById(int id);
        Task Add(Event events);
        Task Update(Event events);
        Task Delete(int id);
        Task<IEnumerable<Event>> GetAllByHashtag(string hastag);
    }
}
