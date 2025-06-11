using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public string? UserId {  get; set; }
        public List<HastagOfBlog> HastagOfBlog { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CategoryBlogId { get; set; }
        [ForeignKey("CategoryBlogId")]
        public CategoryBlog? CategoryBlog { get; set; }
        public int? IsAccept { get; set; } // 0 là đang chờ xét duyệt, 1 là duyệt, 2 là từ chối
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<Rate>? Rate { get; set; } = new List<Rate>();
        public ICollection<Notification>? Notifications { get; set; } = new List<Notification>();

    }
}
