using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class FavoriteBlog
    {
        public int Id { get; set; }
       

        public string? UserId { get; set; }
        public int? BlogId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [ForeignKey("BlogId")]
        public Blog? Blog { get; set; }
    }
}
