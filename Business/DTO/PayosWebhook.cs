using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class PayosWebhook
    {
        public string Code { get; set; }               // Trạng thái: PAID, FAILED, CANCELLED...
        public string Description { get; set; }        // Mô tả trạng thái
        public long OrderCode { get; set; }            // Mã đơn hàng bạn đã tạo
        public int Amount { get; set; }                // Số tiền thanh toán (đơn vị: VND)
        public string UpdatedAt { get; set; }
    }
}
