using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class CommentVideo
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
       
        public DateTime CommentDate { get; set; }

        public string? UserId { get; set; }
        public int? VideoId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [ForeignKey("VideoId")]
        public Video? Video { get; set; }
    }
}
