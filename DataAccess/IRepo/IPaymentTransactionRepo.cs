using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IPaymentTransactionRepo
    {
        Task<IEnumerable<PaymentTransaction>> GetAll();
        Task<IEnumerable<PaymentTransaction>> GetAllByUserId(string userId);
        Task<PaymentTransaction> GetById(int id);
        Task Add(PaymentTransaction events);
        Task Update(PaymentTransaction events);
        Task Delete(int id);
        Task UpdateTransactionStatusAsync(string orderCode, string status);
        Task<PaymentTransaction?> GetTransactionByOrderCodeAsync(string orderCode);



    }
}
