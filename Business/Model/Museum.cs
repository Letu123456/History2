using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Museum
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string EstablishYear {  get; set; }
        public string Contact {  get; set; }
        public List<MuseumImage> Images { get; set; }
        public string Image { get; set; }
        public string Video {  get; set; }
        public ICollection<Artifact>? Artifacts { get; set; } = new List<Artifact>();
        public ICollection<Event>? Events { get; set; } = new List<Event>();
        public User? User {  get; set; }


        public string GetContext()
        {
            return $"Tôi là chatbot của bảo tàng {Name}. Đây là thông tin về bảo tàng: " +
                   $"Tên: {Name}, Mô tả: {Description}, Vị trí: {Location}, " +
                   $"Năm thành lập: {EstablishYear}, Liên hệ: {Contact}, " +
                   $"Số hiện vật: {Artifacts.Count}, Số sự kiện: {Events.Count}. " +
                   "Tôi chỉ trả lời các câu hỏi liên quan đến bảo tàng này.";
        }
    }
}
