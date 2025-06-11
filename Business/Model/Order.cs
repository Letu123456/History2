using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string? UserId {  get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status {  get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ShippingInfor>? ShippingInfors { get; set; } = new List<ShippingInfor>();
        public ICollection<PaymentTransactionProduct>? PaymentTransactionProducts { get; set; } = new List<PaymentTransactionProduct>();


    }
}
