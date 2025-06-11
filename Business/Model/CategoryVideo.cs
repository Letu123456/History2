using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class CategoryVideo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Video>? Video { get; set; } = new List<Video>();
    }
}
