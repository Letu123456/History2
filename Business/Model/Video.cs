using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Video
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoClip{ get; set; }
        public string Music { get; set; }
        public string? UserId {  get; set; }
        public string Source {  get; set; }
        [ForeignKey("UserId")]
        public User? User {  get; set; }
        public int? CategoryVideoId {  get; set; }
        [ForeignKey("CategoryVideoId")]
        public CategoryVideo? CategoryVideo { get; set; }

        public ICollection<CommentVideo>? CommentVideo { get; set; } = new List<CommentVideo>();

    }
}
