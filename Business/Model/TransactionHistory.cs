using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string PremiumPlan { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
