using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class HastagOfBlog
    {
        public int Id { get; set; }
        public string Hashtag { get; set; }

        public int? BlogId { get; set; }

        [ForeignKey("BlogId")]
        public Blog? Blog { get; set; }
    }
}
