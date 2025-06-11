using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business.Model
{
    public class PaymentTransactionProduct
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string CheckoutUrl { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
    }
}
