using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public int? ProdutId { get; set; }
        [ForeignKey("ProdutId")]
        public Product? Product { get; set; }
        public int Quatity {  get; set; }
        public decimal PriceAtPurchase {  get; set; }
    }
}
