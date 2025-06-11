using Business.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class GetOrderDto
    {
        public string OrderCode { get; set; }
        public string? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
    }
}
