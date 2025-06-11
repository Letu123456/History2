using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ShippingInforDto
    {
        public int OrderId {  get; set; }
        public string Address { get; set; }
        public string PhoneNumber {  get; set; }
        public string DeliveryStatus {  get; set; }
    }
}
