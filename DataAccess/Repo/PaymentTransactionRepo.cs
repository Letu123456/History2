using Business.Model;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.IRepo;
using Microsoft.EntityFrameworkCore;
using DataAccess.Service;
using Newtonsoft.Json;

namespace DataAccess.Repo
{
    public class PaymentTransactionRepo:IPaymentTransactionRepo
    {
        private readonly AppDbContext _context;
        private readonly PayOsService _payOsService;

        public PaymentTransactionRepo(AppDbContext context, PayOsService payOsService)
        {
            _context = context;
            _payOsService = payOsService;
        }

        public async Task Add(PaymentTransaction cateBlog)
        {
            await _context.paymentTransactions.AddAsync(cateBlog);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateBlog = await GetById(id);
            if (cateBlog != null)
            {
                _context.paymentTransactions.Remove(cateBlog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PaymentTransaction>> GetAll()
        {
            return await _context.paymentTransactions.ToListAsync();
        }

        public async Task<PaymentTransaction> GetById(int id)
        {
            var cateBlog = await _context.paymentTransactions.FirstOrDefaultAsync(o => o.Id == id);
            if (cateBlog == null)
            {
                return null;
            }

            return cateBlog;
        }

        public async Task Update(PaymentTransaction cateBlog)
        {
            var exisItem = await GetById(cateBlog.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateBlog);
            }
            else
            {
                await _context.paymentTransactions.AddAsync(cateBlog);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PaymentTransaction>> GetAllByUserId(string userId)
        {
            var transactions = await _context.paymentTransactions
                .Include(x => x.User)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            foreach (var transaction in transactions)
            {
                try
                {
                    // 🔁 Gọi API PayOS để lấy trạng thái thật sự
                    var resultJson = await _payOsService.GetPaymentStatusAsync(transaction.OrderCode);
                    dynamic statusPayload = JsonConvert.DeserializeObject(resultJson);

                    string actualStatus = statusPayload?.data?.status?.ToString()?.ToUpper();

                    if (!string.IsNullOrEmpty(actualStatus) && transaction.Status != actualStatus)
                    {
                        transaction.Status = actualStatus;
                    }
                }
                catch (Exception ex)
                {
                    // 🔧 Có thể log lỗi hoặc bỏ qua đơn hàng lỗi
                    Console.WriteLine($"Lỗi cập nhật trạng thái cho orderCode {transaction.OrderCode}: {ex.Message}");
                }
            }

            // 💾 Lưu lại các thay đổi nếu có
            await _context.SaveChangesAsync();

            return transactions;
        }

        public async Task UpdateTransactionStatusAsync(string orderCode, string status)
        {
            var transaction = await _context.paymentTransactions.FirstOrDefaultAsync(t => t.OrderCode == orderCode);
            if (transaction != null)
            {
                transaction.Status = status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PaymentTransaction?> GetTransactionByOrderCodeAsync(string orderCode)
        {
            return await _context.paymentTransactions
                                 .FirstOrDefaultAsync(t => t.OrderCode == orderCode);
        }

    }
}
