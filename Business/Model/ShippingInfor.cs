using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ShippingInfor
    {
        public int Id {  get; set; }
        public int? OrderId {  get; set; }
        public string Address { get; set;}
        public string PhoneNumber { get; set;}
        public string DeliveryStatus {  get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

    }
}
