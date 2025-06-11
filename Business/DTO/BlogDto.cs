using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class BlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        
        public int? CategoryBlogId {  get; set; }

        

        
    }
}
