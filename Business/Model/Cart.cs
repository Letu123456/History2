using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
    }
}
