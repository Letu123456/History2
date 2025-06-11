using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Notification
    {
        public int Id { get; set; }
        public string? ReceiverNotiId { get; set; } // Người nhận thông báo
        public string? SenderNotiId {  get; set; }
        public int? BlogId { get; set; }
        public string Message { get; set; } // Nội dung thông báo
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false; // Đã đọc hay chưa
        [ForeignKey("ReceiverNotiId")]
        public User? ReceiverNoti { get; set; } // Quan hệ với User
        [ForeignKey("SenderNotiId")]
        public User? SenderNoti { get; set; } // Quan hệ với User
        [ForeignKey("BlogId")]
        public Blog? Blog { get; set; }

    }
}
