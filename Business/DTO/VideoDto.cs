using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class VideoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Music { get; set; }
        public IFormFile VideoClip { get; set; }
        public int? CategoryId { get; set; }
        
    }
}
