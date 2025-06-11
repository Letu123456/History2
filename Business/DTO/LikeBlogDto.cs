using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class LikeBlogDto
    {
        public string? ReceiverNotiId { get; set; } // Người nhận thông báo
        public string? SenderNotiId { get; set; } //Người gởi
        public int? BlogId { get; set; }
        public string Message { get; set; } // Nội dung thông báo
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false; // Đã đọc hay chưa
    }
}
