using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }

        [Required]
        public int? CategoryProductId { get; set; }

        [ForeignKey("CategoryProductId")]
        public CategoryProduct? CategoryProduct { get; set; }

        public ICollection<RedemptionHistory> RedemptionHistories { get; set; } = new List<RedemptionHistory>();
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();

    }
}
