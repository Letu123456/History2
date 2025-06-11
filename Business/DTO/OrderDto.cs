using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class OrderDto
    {
        public string OrderCode {  get; set; }
        public decimal TotalAmount { get; set; }
    }
}
