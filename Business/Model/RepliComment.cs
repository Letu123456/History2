using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class RepliComment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public DateTime CommentDate { get; set; }

        public string? UserId { get; set; }
        public int? EventId { get; set; }
        public int? CommentId {  get; set; } 
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [ForeignKey("EventId")]
        public Event? Event { get; set; }
        [ForeignKey("CommentId")]
        public Comment? Comment { get; set; }
    }
}
