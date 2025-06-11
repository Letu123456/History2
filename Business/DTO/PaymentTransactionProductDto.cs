using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class PaymentTransactionProductDto
    {
        public string OrderCode { get; set; }
        public string CheckoutUrl { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public string? UserId { get; set; }
        
        public int? OrderId { get; set; }
        
        public Order? Order { get; set; }
    }
}
