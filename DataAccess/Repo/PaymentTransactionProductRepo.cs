using Business.Model;
using Business;
using DataAccess.IRepo;
using DataAccess.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Business.DTO;

namespace DataAccess.Repo
{
    public class PaymentTransactionProductRepo:IPaymentTransactionProductRepo
    {
        private readonly AppDbContext _context;
        private readonly PayOsService _payOsService;

        public PaymentTransactionProductRepo(AppDbContext context, PayOsService payOsService)
        {
            _context = context;
            _payOsService = payOsService;
        }

        public async Task Add(PaymentTransactionProduct cateBlog)
        {
            await _context.paymentTransactionProducts.AddAsync(cateBlog);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cateBlog = await GetById(id);
            if (cateBlog != null)
            {
                _context.paymentTransactionProducts.Remove(cateBlog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PaymentTransactionProduct>> GetAll()
        {
            return await _context.paymentTransactionProducts.ToListAsync();
        }

        public async Task<PaymentTransactionProduct> GetById(int id)
        {
            var cateBlog = await _context.paymentTransactionProducts.FirstOrDefaultAsync(o => o.Id == id);
            if (cateBlog == null)
            {
                return null;
            }

            return cateBlog;
        }

        public async Task Update(PaymentTransactionProduct cateBlog)
        {
            var exisItem = await GetById(cateBlog.Id);
            if (exisItem != null)
            {
                _context.Entry(exisItem).CurrentValues.SetValues(cateBlog);
            }
            else
            {
                await _context.paymentTransactionProducts.AddAsync(cateBlog);
            }
            await _context.SaveChangesAsync();
        }
        //public async Task<IEnumerable<PaymentTransactionProductDto>> GetAllByUserId(string userId)
        //{
        //    var transactions = await _context.paymentTransactionProducts
        //        .Include(x => x.Order)
        //        .Where(o => o.UserId == userId)
        //        .ToListAsync();

        //    foreach (var transaction in transactions)
        //    {
        //        try
        //        {
        //            // 🔁 Gọi API PayOS để lấy trạng thái thật sự
        //            var resultJson = await _payOsService.GetPaymentStatusAsync(transaction.OrderCode);
        //            dynamic statusPayload = JsonConvert.DeserializeObject(resultJson);

        //            string actualStatus = statusPayload?.data?.status?.ToString()?.ToUpper();

        //            if (!string.IsNullOrEmpty(actualStatus) && transaction.Status != actualStatus)
        //            {
        //                transaction.Status = actualStatus;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // 🔧 Có thể log lỗi hoặc bỏ qua đơn hàng lỗi
        //            Console.WriteLine($"Lỗi cập nhật trạng thái cho orderCode {transaction.OrderCode}: {ex.Message}");
        //        }
        //    }

        //    // 💾 Lưu lại các thay đổi nếu có
        //    await _context.SaveChangesAsync();

        //    return transactions.Select(x => new PaymentTransactionProductDto
        //    {
        //        OrderCode = x.OrderCode,
        //        Status = x.Status,
        //        CheckoutUrl=x.CheckoutUrl,
        //        TotalPrice=x.TotalPrice,
        //        Created=x.Created,
        //        UserId=x.UserId,
        //        OrderId=x.OrderId,
        //        Order = x.Order == null ? null : new GetOrderDto
        //        {
        //            OrderCode=x.OrderCode,
        //            Status = x.Order.Status,
        //            OrderItems=x.Or
        //        }
        //    });
        //}
        public async Task<IEnumerable<PaymentTransactionProduct>> GetAllByUserId(string userId)
        {
            var transactions = await _context.paymentTransactionProducts
                .Include(x => x.Order)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            foreach (var transaction in transactions)
            {
                try
                {
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
                    Console.WriteLine($"Lỗi cập nhật trạng thái cho orderCode {transaction.OrderCode}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return transactions;
            
        }

        

        public async Task UpdateTransactionStatusAsync(string orderCode, string status)
        {
            var transaction = await _context.paymentTransactionProducts.FirstOrDefaultAsync(t => t.OrderCode == orderCode);
            if (transaction != null)
            {
                transaction.Status = status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PaymentTransactionProduct?> GetTransactionByOrderCodeAsync(string orderCode)
        {
            return await _context.paymentTransactionProducts.Include(x=>x.User).FirstOrDefaultAsync(t => t.OrderCode == orderCode);
        }
    }
}
