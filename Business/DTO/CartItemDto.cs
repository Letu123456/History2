using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class CartItemDto
    {
        public int cartId {  get; set; }
        public int productId { get; set; }
        public int quatity {  get; set; }
    }
}
