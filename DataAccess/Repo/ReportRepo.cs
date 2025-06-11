using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repo
{
    public class ReportRepo:IReportRepo
    {
        private AppDbContext _context;

        public ReportRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Report report)
        {
            await _context.reports.AddAsync(report);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var report = await GetById(id);
            if (report != null)
            {
                _context.reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Report>> GetAll()
        {
            return await _context.reports.Include(x => x.User).ToListAsync();
        }

        public async Task<Report> GetById(int id)
        {
            var report = await _context.reports.Include(x => x.User).FirstOrDefaultAsync(o => o.Id == id);
            if (report == null)
            {
                return null;
            }

            return report;
        }

        public async Task Update(Report report)
        {
            var exisItem = await GetById(report.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(report);
            }
            else
            {
                await _context.reports.AddAsync(report);
            }
            await _context.SaveChangesAsync();
        }
    }
}
