using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IPaymentTransactionProductRepo
    {
        Task<IEnumerable<PaymentTransactionProduct>> GetAll();
        Task<IEnumerable<PaymentTransactionProduct>> GetAllByUserId(string userId);
        Task<PaymentTransactionProduct> GetById(int id);
        Task Add(PaymentTransactionProduct events);
        Task Update(PaymentTransactionProduct events);
        Task Delete(int id);
        Task UpdateTransactionStatusAsync(string orderCode, string status);
        Task<PaymentTransactionProduct?> GetTransactionByOrderCodeAsync(string orderCode);
    }
}
