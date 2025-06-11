using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ProductDto
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }
        public int Stock { get; set; }
        public IFormFile Image { get; set; }
        public int? CategoryProductId { get; set; }
    }
}
